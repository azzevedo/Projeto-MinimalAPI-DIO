using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using Test.Infraestrutura.Db;

namespace Test.Dominio.Servicos;

[TestClass]
public class AdministradorServicoTest
{
	/// Contexto de teste em memória com dados pré-carregados <br/>
	/// Herda de DbContexto
	/// DbContextoTest _context = default!;
	DbContexto _context = default!;
	AdministradorServico _servico = default!;

	[TestInitialize]
	public async Task Setup()
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

		_servico = new AdministradorServico(_context);
	}

	/// <summary>
	/// Testa validar login de administradores já cadastrados nos dados pré-carregados
	/// </summary>
	[TestMethod]
	public void ValidarLoginAdministradoresCadastradosComSucesso()
	{
		// Arrange
		LoginDTO loginA = new()
		{
			Email = "adm@teste.com",
			Senha = "1234asdf"
		};

		LoginDTO loginB = new()
		{
			Email = "zze@zze.com",
			Senha = "asdfqwer1234"
		};

		LoginDTO loginC = new()
		{
			Email = "master@teste.com",
			Senha = "1234asdf"
		};

		LoginDTO loginD = new()
		{
			Email = "zze@teste.com",
			Senha = "12a34asdf"
		};

		// Act
		bool validoA = _servico.IsValidLogin(loginA, out Administrador adm);
		bool validoB = _servico.IsValidLogin(loginB, out Administrador zze);
		bool validoC = _servico.IsValidLogin(loginC, out Administrador master);
		bool validoD = _servico.IsValidLogin(loginD, out Administrador zzeTeste);

		// Assert
		Assert.IsTrue(validoA);
		Assert.AreEqual("adm@teste.com", adm.Email);
		// Console.WriteLine(adm.Perfil);

		Assert.IsTrue(validoB);
		Assert.AreEqual("zze@zze.com", zze.Email);
		// Console.WriteLine(zze.Perfil);

		Assert.IsTrue(validoC);
		Assert.AreEqual("master@teste.com", master.Email);
		// Console.WriteLine(master.Perfil);

		Assert.IsTrue(validoD);
		Assert.AreEqual("zze@teste.com", zzeTeste.Email);
		// Console.WriteLine(zzeTeste.Perfil);
	}

	[TestMethod]
	public void TestLoginInvalido()
	{
		// Arrange
		LoginDTO loginInvalido1 = new()
		{
			Email = "inv@inv.com",
			Senha = "senhaerrada"
		};

		LoginDTO loginInvalido2 = new()
		{
			Email = "outro@email.com",
			Senha = "123456"
		};

		// Act
		bool valido1 = _servico.IsValidLogin(loginInvalido1, out Administrador adm1);
		bool valido2 = _servico.IsValidLogin(loginInvalido2, out Administrador adm2);

		// Assert
		Assert.IsFalse(valido1);
		Assert.AreEqual(0, adm1.Id); // adm vazio
		Assert.IsFalse(valido2);
		Assert.AreEqual(0, adm2.Id); // adm vazio
	}

	/// <summary>
	/// Testa salvar um novo administrador e depois logar com ele usando o Sqlite em memória
	/// </summary>
	[TestMethod]
	public void TestIncluirELogarAdm()
	{
		// Arrange
		Administrador adm = new()
		{
			Email = "angels@cry.com",
			Senha = "angra",
			Perfil = "editor"
		};

		// Tentar logar com o novo adm - LoginDTO para validação de login
		LoginDTO login = new()
		{
			Email = adm.Email,
			Senha = adm.Senha
		};

		// Act
		// var db = new DbContexto();
		_servico.Incluir(adm).Wait();
		int cadastrados = _context.Administradores.Count();

		bool valido = _servico.IsValidLogin(login, out Administrador admLogado);

		// Assert
		Assert.IsTrue(cadastrados > 0); // 4 já existem pelos dados pré-carregados

		Assert.IsTrue(valido);
		Assert.AreEqual(adm.Email, admLogado.Email);
		Assert.AreEqual(adm.Perfil, admLogado.Perfil);
		Assert.AreEqual(5, admLogado.Id);

		// Output
		Console.WriteLine($"Adms cadastrados: {cadastrados}");
		Console.WriteLine($"Adm salvo: {admLogado.Email}");
	}

	[TestMethod]
	public void TestBuscaPorID()
	{
		// Arrange
		int idExistente = 2; // Zze
		int idInexistente = 999;

		// Act
		var admExistente = _servico.GetAdministradorByIdAsync(idExistente).Result;
		var admInexistente = _servico.GetAdministradorByIdAsync(idInexistente).Result;

		// Assert
		Assert.IsNotNull(admExistente);
		Assert.AreEqual("zze@zze.com", admExistente!.Email);
		Assert.IsNull(admInexistente);
	}


	[TestCleanup]
	public async Task Cleanup()
	{
		await _context.Database.CloseConnectionAsync();
		await _context.DisposeAsync();
	}
}
