using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.DTO;
using Xunit.Abstractions;

namespace EndpointsTestXUnit.Endpoints.Veiculos;

public class TestPostVeiculo(ITestOutputHelper output) : ApiTestBase
{
	/*
	[x] (Unauthorized) - Sem login/autenticação
	(Forbidden) - Editor
	(Created - BadRequest) - Adm
	*/
	readonly string endpoint = "/veiculos";

	[Fact]
	public async Task PostVeiculo_Unauthorized()
	{
		var resp = await client.PostAsJsonAsync(endpoint, new VeiculoDTO());
		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task TestPost_Editor(string email, string senha)
	{
		await Autenticar(email, senha);
		var resp = await client.PostAsJsonAsync(endpoint, new VeiculoDTO());

		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	public async Task TestPost_ADM_BadRequest(string email, string senha)
	{
		await Autenticar(email, senha);
		var veiculo = new VeiculoDTO();
		var resp = await client.PostAsJsonAsync(endpoint, veiculo);

		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

		string content = await resp.Content.ReadAsStringAsync();
		output.WriteLine(content);

		// Apenas informação de ano
		veiculo.Ano = 2000;
		resp = await client.PostAsJsonAsync(endpoint, veiculo);
		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

		content = await resp.Content.ReadAsStringAsync();
		output.WriteLine(content);

		// Ano inferior a 1960
		veiculo.Ano = 1955;
		veiculo.Nome = "scort";
		veiculo.Marca = "ford";
		resp = await client.PostAsJsonAsync(endpoint, veiculo);
		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

		content = await resp.Content.ReadAsStringAsync();
		output.WriteLine(content);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	public async Task TestPost_ADM_OK(string email, string senha)
	{
		await Autenticar(email, senha);
		var veiculo = new VeiculoDTO { Nome = "Brasília Amarela", Marca = "VW", Ano = 1996 };
		var resp = await client.PostAsJsonAsync(endpoint, veiculo);

		Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
	}
}
