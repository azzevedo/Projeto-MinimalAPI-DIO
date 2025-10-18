using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db;

public class DbContexto(DbContextOptions<DbContext> options) : DbContext(options)
{
	public DbSet<Administrador> Administradores { get; set; } = default!;
	public DbSet<Veiculo> Veiculos { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Administrador>().HasData(
			new Administrador()
			{
				Id = 1,
				Email = "adm@teste.com",
				Senha = "1234asdf",
				Perfil = "adm"
			}
		);
	}
}
