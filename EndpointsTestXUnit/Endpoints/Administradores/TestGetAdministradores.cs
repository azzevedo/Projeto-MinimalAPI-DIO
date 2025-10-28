using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.ModelViews;

namespace EndpointsTestXUnit.Endpoints.Administradores;

public class TestGetAdministradores : ApiTestBase
{
	[Fact]
	public async Task Test_GetAdmins_SemAutenticacao_Unauthorized()
	{
		var response = await client.GetAsync("/admins");
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	/// <summary>
	/// Administradores - Perfil.adm
	/// </summary>
	/// <param name="email"></param>
	/// <param name="senha"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	// [InlineData("vira@vira.com", "valeumnamão_monoteta")] // Não existe
	public async Task Test_GetAdmins_Autenticados_OK(string email, string senha)
	{
		/*
		CÓDIGO ANTIGO APENAS PARA REFERÊNCIA
		
		Login
		var dto = new AdministradorDTO { Email = email, Senha = senha };
		var loginResp = await client.PostAsJsonAsync("/adm/login", dto);
		loginResp.EnsureSuccessStatusCode();

		var admLogado = await loginResp.Content.ReadFromJsonAsync<AdmLogado>()
				?? throw new InvalidOperationException("Sem token");

		// if (!json.TryGetProperty("token", out JsonElement tkn))
		// 	throw new InvalidOperationException("Sem token");
		// token = tkn.GetString() ?? string.Empty;

		// Chamada autenticada
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogado.Token);

		var resp = await client.GetAsync("/admins");
		*/
		var resp = await LogarUser(email, senha);
		Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

		var admModelView = resp.Content.ReadFromJsonAsAsyncEnumerable<AdministradorModelView>();
		var listaAdms = admModelView.ToBlockingEnumerable().ToList();

		// Há 4 adms definidos em ApiTestBase + 1 em DbContexto
		Assert.Equal(5, listaAdms.Count);
	}

	/// <summary>
	/// Login com sucesso, mas não pode receber resposta do endpoint por causa do perfil (editor)
	/// </summary>
	/// <param name="email"></param>
	/// <param name="senha"></param>
	/// <returns></returns>
	[Theory]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task Test_GetAdmins_Editores_Falha(string email, string senha)
	{
		var resp = await LogarUser(email, senha);
		Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
	}

	/// <summary>
	/// Helper para logar e autenticar com token de AdmLogado <br/>
	/// Retorna HttpResponseValida para ADM, inválida para EDITOR
	/// </summary>
	/// <param name="email"></param>
	/// <param name="senha"></param>
	/// <returns>HttpResponseMessage</returns>
	/// <exception cref="InvalidOperationException"></exception>
	async Task<HttpResponseMessage> LogarUser(string email, string senha)
	{
		/*
		var dto = new AdministradorDTO { Email = email, Senha = senha };
		// var loginResp = await client.PostAsJsonAsync("/adm/login", dto);
		// loginResp.EnsureSuccessStatusCode();

		// var admLogado = await loginResp.Content.ReadFromJsonAsync<AdmLogado>()
		// 		?? throw new InvalidOperationException("Sem token");

		// client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", admLogado.Token);
		*/
		// var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		// client.DefaultRequestHeaders.Authorization = auth;
		await Autenticar(email, senha);

		var resp = await client.GetAsync("/admins");
		return resp;
	}
}
