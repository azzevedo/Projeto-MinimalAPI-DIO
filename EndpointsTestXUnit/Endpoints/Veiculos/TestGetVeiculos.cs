using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.Entidades;

namespace EndpointsTestXUnit.Endpoints.Veiculos;

public class TestGetVeiculos : ApiTestBase, IUserLogger
{
	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task GetVeiculos(string email, string senha)
	{
		string endpoint = $"/veiculos?pagina=1";
		await Autenticar(email, senha);

		var resp = await client.GetAsync(endpoint);
		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

		var veiculos = await resp.Content.ReadFromJsonAsync<List<Veiculo>>();
		Assert.NotNull(veiculos);
		Assert.Equal(3, veiculos.Count); // Dados iniciais em ApiTestBase
	}

	async Task Autenticar(string email, string senha)
	{
		var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		client.DefaultRequestHeaders.Authorization = auth;
	}
}
