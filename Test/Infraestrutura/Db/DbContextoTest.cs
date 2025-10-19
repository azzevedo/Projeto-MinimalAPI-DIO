using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;

namespace Test.Infraestrutura.Db;

public class DbContextoTest(DbContextOptions<DbContexto> options) : DbContexto(options)
{
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Administrador>().HasData(
			new Administrador()
			{
				Id = 1,
				Email = "adm@teste.com",
				Senha = "1234asdf",
				Perfil = "adm"
			},
			new Administrador()
			{
				Id = 2,
				Email = "zze@zze.com",
				Senha = "asdfqwer1234",
				Perfil = "adm"
			},
			new Administrador()
			{
				Id = 3,
				Email = "master@teste.com",
				Senha = "1234asdf",
				Perfil = "editor"
			},
			new Administrador()
			{
				Id = 4,
				Email = "zze@teste.com",
				Senha = "12a34asdf",
				Perfil = "editor"
			}
		);
	}
}
