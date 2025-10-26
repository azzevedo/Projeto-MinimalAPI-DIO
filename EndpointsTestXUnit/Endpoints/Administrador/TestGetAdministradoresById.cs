using System.Net;
using EndpointsTestXUnit.Helpers;
using Xunit.Abstractions;

namespace EndpointsTestXUnit.Endpoints.Administrador;

public class TestGetAdministradoresById(ITestOutputHelper output) : ApiTestBase, IUserLogger
{
	/* [x] SUCESSO */
	// Fazer login autenticado - adm
	// Pegar por id
	// 	- user existe - RESULTS.OK
	//  - user NÃO EXISTE - RESULTS.NOTFOUND 

	/* [x] FALHA */
	// Fazer login autenticado - editor
	// pegar por id - FORBIDDEN 

	/* [x] SEM AUTENTICAÇÃO - UNAUTHORIZED*/

	[Theory]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task Test_GetAdminsById_Forbidden(string email, string senha)
	{
		await Autenticar(email, senha);

		var resp = await client.GetAsync("/admins/1"); // Pegar user com id 1 (existente)
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	[Fact]
	public async Task Test_GetAdminsById_Unauthorized()
	{
		var resp = await client.GetAsync("/admins/1"); // Tentar pegar user com id 1 (existente)
		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 2)]
	[InlineData("mundo@animal.com", "dornascostas", 3)]
	[InlineData("adm@teste.com", "1234asdf", 5)]
	public async Task Test_GetAdminsById_OK_NotFound(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		var resp = await client.GetAsync($"/admins/{id}"); // Pegar user com ids variados (existentes)
		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

		string str = await resp.Content.ReadAsStringAsync();
		output.WriteLine(str);

		// NOT FOUND
		resp = await client.GetAsync("/admins/42"); // não existe
		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}

	async Task Autenticar(string email, string senha)
	{
		var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		client.DefaultRequestHeaders.Authorization = auth;
	}
}

