using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestrutura.Db;

public class DbContexto(IConfiguration configuration) : DbContext
{
	public DbSet<Administrador> Administradores { get; set; } = default!;
	public DbSet<Veiculo> Veiculos { get; set; } = default!;

	readonly string? _stringConexao = configuration.GetConnectionString("mysql");
	readonly IConfiguration _configuracaoAppSettings = configuration;

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.IsConfigured) return;
		if (string.IsNullOrEmpty(_stringConexao)) return;

		optionsBuilder.UseMySql(
			_stringConexao, ServerVersion.AutoDetect(_stringConexao)
		);
	}

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
