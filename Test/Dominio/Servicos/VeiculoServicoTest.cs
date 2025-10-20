using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;

namespace Test.Dominio.Servicos;

[TestClass]
public class VeiculoServicoTest : TestBase
{
	VeiculoServico _servico = default!;

	[TestInitialize]
	public async Task Setup()
	{
		await CriarContextoEmMemoria();
		_servico = new VeiculoServico(_context);
	}

	[TestMethod]
	public async Task TestInsertVeiculo()
	{
		// Arrange
		var veiculo = new Veiculo
		{
			Nome = "TesteVeiculo",
			Marca = "TesteMarca",
			Ano = 2024
		};

		// Act
		await _servico.InsertVeiculo(veiculo);
		var result = await _servico.GetVeiculoById(veiculo.Id);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual("TesteVeiculo", result.Nome);
		Assert.AreEqual(2024, result.Ano);
	}

	[TestMethod]
	public async Task TestGetVeiculoById_Falha()
	{
		// Act
		var veiculo = await _servico.GetVeiculoById(1); // ID que não existe

		// Assert
		Assert.IsNull(veiculo);
	}

	// TODO
	[TestMethod]
	public async Task TestGetVeiculoById_Sucesso()
	{
		// Arrange
		await AddVeiculo(3); // Adiciona um veículo para garantir que há dados

		// Act
		var veiculo = await _servico.GetVeiculoById(2); // ID do veículo adicionado

		// Assert
		Assert.IsNotNull(veiculo);
	}

#region Métodos auxiliares
	public async Task AddVeiculo(int quantidade = 1)
	{
		for (int i = 1; i <= quantidade; i++)
		{
			await _servico.InsertVeiculo(new Veiculo
			{
				Nome = $"Veiculo{i}",
				Marca = $"Marca{i}",
				Ano = 2000 + i,
			});
		}
	}

	[TestCleanup]
	public async Task Cleanup()
	{
		await FecharContextoEmMemoria();
	}
#endregion
}
