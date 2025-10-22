// using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.Db;
using Microsoft.VisualStudio.TestPlatform.TestHost;
// using Test.Infraestrutura.Db;
using minimal_api.Dominio.Entidades;

namespace Test.Requests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	SqliteConnection _connection = default!;

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Testing");

		builder.ConfigureServices(services =>
		{
			// Remove o DbContext configurado para o ambiente de produção (MySQL)
			var descriptor = services.SingleOrDefault(
				d => d.ServiceType == typeof(DbContextOptions<DbContexto>)
			);

			if (descriptor != null)
				services.Remove(descriptor);

			// Cria uma conexão SQLite em memória
			_connection = new SqliteConnection("DataSource=:memory:");
			_connection.OpenAsync().Wait();

			// Adiciona o DbContext usando SQLite em memória == registra o novo DbContext
			// services.AddDbContext<DbContextoTest>(options =>
			services.AddDbContext<DbContexto>(options =>
			{
				options.UseSqlite(_connection);
			});

			// Garante que o banco de dados seja criado
			using var scope = services.BuildServiceProvider().CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<DbContexto>();
			db.Database.EnsureCreatedAsync().Wait();



			if (!db.Administradores.Any())
			{
				// Popula o banco de dados com dados de teste
				db.Administradores.AddRange(
					new Administrador()
					{
						Id = 1,
						Email = "teste@http.com",
						Senha = "1234asdf",
						Perfil = "adm"
					},

					new Administrador()
					{
						Id = 2,
						Email = "http@teste.com",
						Senha = "asdfqwer1234",
						Perfil = "editor"
					}
				);

				db.SaveChangesAsync().Wait();
			}
		});
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		_connection?.DisposeAsync().AsTask().Wait();
	}
}
/*
	options.AddSecurityRequirement(
			new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme()
			{
				Reference = new OpenApiReference()
				{
					Type = ReferenceType.SecurityScheme,
					Id = "bearer"
				}
			},
					new string[] { }
				}
	}
		);
	});
}
*/