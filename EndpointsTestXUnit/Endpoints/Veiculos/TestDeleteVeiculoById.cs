using System.Net;
using EndpointsTestXUnit.Helpers;

namespace EndpointsTestXUnit.Endpoints.Veiculos;

public class TestDeleteVeiculoById : ApiTestBase
{
	/* ADM têm permissão
	Not Found ou OK

	Unauthorized
	Forbidden (Editor)
	*/
	readonly string endpoint = "/veiculos/";

	[Fact]
	public async Task DeleteVeiculoById_Unauthorized()
	{
		int id = 10;

		var resp = await client.DeleteAsync(endpoint + id);
		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas", 2)]
	[InlineData("adm@teste.com", "1234asdf", 3)]
	public async Task DeleteVeiculosById_NotFound_OK(string email, string senha, int id = 1)
	{
		await Autenticar(email, senha);
		var resp = await client.DeleteAsync(endpoint + id);
		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

		resp = await client.DeleteAsync(endpoint + (id + 42));
		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta", 34)]
	[InlineData("lavem@alemao.com", "facade2legumes", 2)]
	public async Task DeleteVeiculosById_Forbidden(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		var resp = await client.DeleteAsync(endpoint + id);
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}
}
