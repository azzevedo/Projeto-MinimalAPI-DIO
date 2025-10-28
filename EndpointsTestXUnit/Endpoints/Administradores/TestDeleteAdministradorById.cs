using System.Net;
using EndpointsTestXUnit.Helpers;

namespace EndpointsTestXUnit.Endpoints.Administradores;

public class TestDeleteAdministradorById : ApiTestBase, IUserLogger
{
	/*
	[x] Editor - FORBIDDEN

	Adm - SUCESSO [OK]
	Adm - NOT FOUND

	[x] Sem login - UNAUTHORIZED 
	*/

	[Fact]
	public async Task Delete_SemLogin()
	{
		int id = 1;

		var resp = await client.DeleteAsync($"/admins/{id}");

		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta", 2)]
	[InlineData("lavem@alemao.com", "facade2legumes", 1)]
	[InlineData("vira@vira.com", "monoteta", 42)]
	public async Task TestDelete_Editor(string email, string senha, int id)
	{
		await Autenticar(email, senha);

		var resp = await client.DeleteAsync($"/admins/{id}");
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 2)]
	[InlineData("mundo@animal.com", "dornascostas", 3)]
	[InlineData("adm@teste.com", "1234asdf", 5)]
	public async Task TestDelete_Adm_OK(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		var resp = await client.DeleteAsync($"/admins/{id}");

		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 42)]
	[InlineData("mundo@animal.com", "dornascostas", 69)]
	[InlineData("adm@teste.com", "1234asdf", 34)]
	public async Task TestDelete_Adm_NotFound(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		var resp = await client.DeleteAsync($"/admins/{id}");

		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}

	
	async Task Autenticar(string email, string senha)
	{
		var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		client.DefaultRequestHeaders.Authorization = auth;
	}
}
