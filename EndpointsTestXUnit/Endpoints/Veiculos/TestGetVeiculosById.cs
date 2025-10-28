using System.Net;
using EndpointsTestXUnit.Helpers;

namespace EndpointsTestXUnit.Endpoints.Veiculos;

public class TestGetVeiculosById : ApiTestBase
{
	/* ADM ou EDITOR pode usar este endpoint

	1. Sem autenticação - UNAUTHORIZED
	2. ADM ou EDITOR
		NOT FOUND
		OK
	*/
	string endpoint = "/veiculos/";

	[Fact]
	public async Task GetVeiculosById_Unauthorized()
	{
		endpoint += 1;  // Id 1

		var resp = await client.GetAsync(endpoint);
		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task GetVeiculosById_NotFound_OK(string email, string senha)
	{
		await Autenticar(email, senha);
		var resp = await client.GetAsync(endpoint + 1); // Id existente
		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

		resp = await  client.GetAsync(endpoint + 8); // Id inexistente
		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}
}
