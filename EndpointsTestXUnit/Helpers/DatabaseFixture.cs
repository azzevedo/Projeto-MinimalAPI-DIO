using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;

namespace EndpointsTestXUnit.Helpers;

public class DatabaseFixture : IAsyncLifetime
{
	public WebApplicationFactory<Program> Factory { get; private set; } = default!;
	public DbContexto Contexto { get; private set; } = default!;

	public async Task InitializeAsync()
	{
		Factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DbContexto>));

					if (descriptor is not null)
						services.Remove(descriptor);

					services.AddDbContext<DbContexto>(options =>
					{
						options.UseSqlite("DataSource=:memory:");
						options.EnableSensitiveDataLogging();
					});
				});
			});

		var scope = Factory.Services.CreateScope();
		Contexto = scope.ServiceProvider.GetRequiredService<DbContexto>();

		await Contexto.Database.OpenConnectionAsync();
		await Contexto.Database.EnsureCreatedAsync();

		await AdicionarDadosCompartilhados();
	}

	private async Task AdicionarDadosCompartilhados()
	{
		Contexto.Administradores.AddRange(
			new Administrador { Email = "mundo@animal.com", Senha = "1234zxcv", Perfil = "editor" },
			new Administrador { Email = "mundo@bagre.com", Senha = "1aaa234zxcv", Perfil = "adm" }
		);

		await Contexto.SaveChangesAsync();
	}


	public async Task DisposeAsync()
	{
		await Contexto.Database.CloseConnectionAsync();
		await Contexto.DisposeAsync();
		await Factory.DisposeAsync();
	}
}


// Collection definition para compartilhar a fixture entre classes de teste
[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture> {}
