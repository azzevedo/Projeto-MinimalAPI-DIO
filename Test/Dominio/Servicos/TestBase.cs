using Microsoft.EntityFrameworkCore;
using minimal_api.Infraestrutura.Db;
using Test.Infraestrutura.Db;


namespace Test.Dominio.Servicos;

/// <summary>
/// Classe base de teste, serve para qualquer serviço que use
/// DbContexto
/// </summary>
public abstract class TestBase
{
	protected DbContexto _context = default!;

	/// <summary>
	/// Cria o contexto em memória com dados pré-carregados
	/// </summary>
	/// <returns></returns>
	protected async Task CriarContextoEmMemoria()
	{
		Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

		var options = new DbContextOptionsBuilder<DbContexto>()
			.UseSqlite("Filename=:memory:")
			.Options;

		// _context = new DbContexto(options);
		/*
		Criar como DbContextoTest para fazer override no OnModelCreating [HasData]
		e popular o banco de dados de teste com dados pré-definidos.
		Mas o _context precisa ser do tipo DbContexto para passar no construtor do AdministradorServico
		*/

		_context = new DbContextoTest(options);
		await _context.Database.OpenConnectionAsync();
		await _context.Database.EnsureCreatedAsync();
	}
}

