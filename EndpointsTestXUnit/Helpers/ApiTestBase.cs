using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;

namespace EndpointsTestXUnit.Helpers;

public abstract class ApiTestBase : IAsyncLifetime
{
	protected HttpClient client = default!;
	protected WebApplicationFactory<Program> factory = default!;
	protected DbContexto db = default!;

	SqliteConnection _connection = default!;

	readonly string env = "Testing";

	public virtual async Task InitializeAsync()
	{
		Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);

		_connection = new SqliteConnection("DataSource=:memory:");
		await _connection.OpenAsync();

		factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment(env);

				builder.ConfigureServices(services =>
				{
					var descriptor = services.SingleOrDefault(
						d => d.ServiceType == typeof(DbContextOptions<DbContexto>)
					);

					if (descriptor is not null)
						services.Remove(descriptor);

					services.AddDbContext<DbContexto>(options =>
					{
						options.UseSqlite(_connection);
						// options.EnableSensitiveDataLogging();
					});

					// var sp = services.BuildServiceProvider();
					// using var scope = sp.CreateScope();
					// db = scope.ServiceProvider.GetRequiredService<DbContexto>();
					// db.Database.OpenConnection();
					// db.Database.EnsureCreated();
					// AdicionarDadosIniciais();
				});
			});

		var scope = factory.Services.CreateScope();
		db = scope.ServiceProvider.GetRequiredService<DbContexto>();

		// await db.Database.OpenConnectionAsync();
		await db.Database.EnsureCreatedAsync();

		await AdicionarDadosIniciais();

		client = factory.CreateClient();

		// await Task.Run(
		// 	() => { client = factory.CreateClient(); }
		// );
	}

	protected virtual async Task AdicionarDadosIniciais()
	{
		//if (!await db.Administradores.AnyAsync())
		var adms = new List<Administrador>
		{ 
			new() { Email = "mamonas@assassinas.com", Senha = "1406", Perfil = "adm" },
			new() { Email = "mundo@animal.com", Senha = "dornascostas", Perfil = "adm" },
			new() { Email = "vira@vira.com", Senha = "monoteta", Perfil = "editor" },
			new() { Email = "lavem@alemao.com", Senha = "facade2legumes", Perfil = "editor" },
		};

		await db.Administradores.AddRangeAsync(adms);

		var veiculos = new List<Veiculo>
		{
			new() { Nome = "Brasília", Marca = "vw", Ano = 1995 },
			new() { Nome = "Fusca", Marca = "vw", Ano = 1969 },
			new() { Nome = "Uno", Marca = "fiat", Ano = 2000 }
		};

		await db.Veiculos.AddRangeAsync(veiculos);

		await db.SaveChangesAsync();
	}

	public virtual async Task DisposeAsync()
	{
		client?.Dispose();

		await _connection.CloseAsync();
		await _connection.DisposeAsync();

		// if (db != null)
		// {
		// 	await db.Database.CloseConnectionAsync();
		// 	await db.DisposeAsync();
		// }

		await factory.DisposeAsync();
	}
}
