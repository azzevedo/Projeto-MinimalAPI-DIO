using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Enum;
using Xunit.Abstractions;

namespace EndpointsTestXUnit.Endpoints.Administradores;

public class TestPutAdministrador(ITestOutputHelper output) : ApiTestBase, IUserLogger
{
	/*
	[x] Sem login -> UNAUTHORIZED
	[x] Perfil.editor -> FORBIDDEN
	Perfil.adm -> [x] NotFound ou OK ou Informações vazias
	*/
	[Fact]
	public async Task Put_SemLogin()
	{
		int id = 1;
		var resp = await client.PutAsJsonAsync($"/admins/{id}", new AdministradorDTO());

		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta", 2)]
	[InlineData("lavem@alemao.com", "facade2legumes", 1)]
	[InlineData("vira@vira.com", "monoteta", 42)]
	public async Task Put_Editor(string email, string senha, int id)
	{
		await Autenticar(email, senha);

		var resp = await client.PutAsJsonAsync($"/admins/{id}", new AdministradorDTO());
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 2)]
	[InlineData("mundo@animal.com", "dornascostas", 3)]
	[InlineData("adm@teste.com", "1234asdf", 5)]
	public async Task Put_Adm_BadRequest(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		string rota = $"/admins/{id}";

		// VAZIO 
		var adm = new AdministradorDTO();

		var resp = await client.PutAsJsonAsync(rota, adm);
		Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

		string content = await resp.Content.ReadAsStringAsync();
		output.WriteLine($"{content}");

		// ALGUNS DADOS
		adm.Email = "roots@bloodyroots.com";
		// Sem senha
		adm.Perfil = (Perfil)63;  // Não existe

		resp = await client.PutAsJsonAsync(rota, adm);
		content = await resp.Content.ReadAsStringAsync();

		output.WriteLine($"{content}");
	}

	// [Theory]
	// [InlineData("mamonas@assassinas.com", "1406", 2)]
	// [InlineData("mundo@animal.com", "dornascostas", 3)]
	// [InlineData("adm@teste.com", "1234asdf", 5)]
	// public async Task Put_Adm_OK(string email, string senha, int id)
	// {
	// 	await Autenticar(email, senha);
	// 	var servico = new AdministradorServico(db);
	// 	var adm = await servico.GetAdministradorByIdAsync(id);
	// 	Assert.Equal()

	// 	var admUpdate = new AdministradorDTO { Email = "chopis@centis.com", Senha = "gergelim" };

	// 	// var resp = await client.PutAsJsonAsync($"/admins/{id}", adm);

	// 	// Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
	// }
	[Theory]
	[InlineData("mamonas@assassinas.com", "1406", 88)]
	[InlineData("mundo@animal.com", "dornascostas", 55)]
	[InlineData("adm@teste.com", "1234asdf", 13)]
	public async Task Put_NotFound(string email, string senha, int id)
	{
		await Autenticar(email, senha);
		var resp = await client.PutAsJsonAsync($"/admins/{id}", new AdministradorDTO());
		Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
	}

	async Task Autenticar(string email, string senha)
	{
		var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		client.DefaultRequestHeaders.Authorization = auth;
	}
}
