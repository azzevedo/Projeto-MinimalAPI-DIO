using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.DTO;
using Xunit.Abstractions;

namespace EndpointsTestXUnit.Endpoints.Veiculos;

public class TestPutVeiculo(ITestOutputHelper output) : ApiTestBase
{
	readonly string endpoint = "/veiculos/";

	[Fact]
	public async Task PutVeiculo_Unauthorized()
	{
		// Sem login
		var resp = await client.PutAsJsonAsync(endpoint + 1, new VeiculoDTO());
		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta", 2)]
	[InlineData("lavem@alemao.com", "facade2legumes", 1)]
	[InlineData("vira@vira.com", "monoteta", 42)]
	public async Task PutVeiculo_Forbidden(string email, string senha, int id = 3)
	{
		// Perfil editor
		await Autenticar(email, senha);

		var resp = await client.PutAsJsonAsync(endpoint + id, new VeiculoDTO());
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 2)]
	[InlineData("mundo@animal.com", "dornascostas", 3)]
	[InlineData("adm@teste.com", "1234asdf", 2)]
	[InlineData("mamonas@assassinas.com", "1406")]
	public async Task PutVeiculo_BadRequest(string email, string senha, int id = 1)
	{
		// INFORMAÇÕES VAZIAS //

		await Autenticar(email, senha);
		string end = endpoint + id;

		// Vazio
		var veiculo = new VeiculoDTO();

		var resp = await client.PutAsJsonAsync(end, veiculo);
		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

		// Alguma informação
		veiculo.Ano = 2000;
		veiculo.Marca = "mamonas";

		resp = await client.PutAsJsonAsync(end, veiculo);
		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
		output.WriteLine(await resp.Content.ReadAsStringAsync());
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 88)]
	[InlineData("mundo@animal.com", "dornascostas", 55)]
	[InlineData("adm@teste.com", "1234asdf", 13)]
	public async Task PutVeiculo_NotFound(string email, string senha, int id)
	{
		// ID INEXISTENTE
		await Autenticar(email, senha);
		var resp = await client.PutAsJsonAsync(endpoint + id, new VeiculoDTO());
		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 1, "Veraneio", "Vascaína", 1988)]
	[InlineData("mundo@animal.com", "dornascostas", 2, "Cadilac", "Bip bip", 2012)]
	[InlineData("adm@teste.com", "1234asdf", 3, "Brasília", "Amarela", 2025)]
	public async Task PutVeiculo_Created(string email, string senha, int id, string nomeVeiculo, string marcaVeiculo, int anoVeiculo)
	{
		string end = endpoint + id;

		var veiculo = new VeiculoDTO() { Nome = nomeVeiculo, Marca = marcaVeiculo, Ano = anoVeiculo };

		await Autenticar(email, senha);
		var resp = await client.PutAsJsonAsync(end, veiculo);

		Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
	}
}
