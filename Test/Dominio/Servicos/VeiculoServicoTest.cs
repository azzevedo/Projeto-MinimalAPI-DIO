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

	[TestMethod]
	public async Task TestUpdateVeiculo_Sucesso()
	{
		// Arrange
		await AddVeiculo(1);

		var veiculo = await _servico.GetVeiculoById(1);

		// Assert
		Assert.IsNotNull(veiculo);

		veiculo.Nome = "VeiculoAtualizado";
		veiculo.Marca = "MarcaAtualizada";
		veiculo.Ano = 2025;

		// Act
		// var v
		var veiculoAtualizado = await _servico.UpdateVeiculo(veiculo);

		// Assert
		Assert.IsNotNull(veiculoAtualizado);
		Assert.AreEqual(veiculo.Nome, veiculoAtualizado.Nome);
		Assert.AreEqual(veiculo.Marca, veiculoAtualizado.Marca);
		Assert.AreEqual(veiculo.Ano, veiculoAtualizado.Ano);
	}

	[TestMethod]
	public async Task TestUpdateVeiculo_Falha()
	{
		// Arrange
		await AddVeiculo(1);

		var veiculo = await _servico.GetVeiculoById(2); // ID que não existe

		// Assert
		Assert.IsNull(veiculo);

		// Act & Assert
		await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
		{
			await _servico.UpdateVeiculo(veiculo!);
		});
	}

	[TestMethod]
	public async Task TestDeleteVeiculo_Sucesso()
	{
		// Arrange
		await AddVeiculo(1);

		var veiculo = await _servico.GetVeiculoById(1);

		// Assert
		Assert.IsNotNull(veiculo);

		// Act
		await _servico.DeleteVeiculo(veiculo);

		var veiculoDeletado = await _servico.GetVeiculoById(1);

		// Assert
		Assert.IsNull(veiculoDeletado);
	}

	[TestMethod]
	public async Task TestGetVeiculos_25Veiculos()
	{
		// Arrange
		await AddVeiculo(25); // Adiciona 25 veículos para teste de paginação

		// Act
		var resultPage1 = await _servico.GetVeiculos(1);
		var resultPage2 = await _servico.GetVeiculos(2);
		var resultPage3 = await _servico.GetVeiculos(3);

		// Assert
		Assert.IsNotNull(resultPage1);
		Assert.IsNotNull(resultPage2);
		Assert.IsNotNull(resultPage3);

		Assert.AreEqual(10, resultPage1.Count); // Página 1 deve ter 10 itens
		Assert.AreEqual(10, resultPage2.Count); // Página 2 deve ter 10 itens
		Assert.AreEqual(5, resultPage3.Count);  // Página 3 deve ter 5 itens

		// Verifica se os veículos retornados são os esperados
		Assert.AreEqual("Veiculo1", resultPage1[0].Nome);
		Assert.AreEqual("Veiculo11", resultPage2[0].Nome);
		Assert.AreEqual("Veiculo21", resultPage3[0].Nome);
	}

	[TestMethod]
	public async Task TestGetVeiculos_5Veiculos()
	{
		// Arrange
		await AddVeiculo(5);

		// Act
		var resultPage1 = await _servico.GetVeiculos(1);
		var resultPage2 = await _servico.GetVeiculos(2);

		// Assert
		Assert.IsNotNull(resultPage1);
		Assert.AreEqual(5, resultPage1.Count); // Deve retornar os 5 veículos
		Assert.IsNotNull(resultPage2);
		Assert.AreEqual(0, resultPage2.Count); // Página 2 deve estar vazia
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
