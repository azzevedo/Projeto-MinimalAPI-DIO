using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Enum;

namespace EndpointsTestXUnit.Endpoints.Administradores;

public class TestPostAdministrador : ApiTestBase, IUserLogger
{
	/*
	[x] - [UNAUTHORIZED]
	acessar endpoint sem fazer login

	[x] [FORBIDDEN]
	login com editor - não pode adicionar novos adms
	login com editor, mas sem usar o token (unauthorized)

	[x] [SUCESSO]
	login com adm + token jwt -> Results.Created
	login com adm - sem token jwt -> Unauthorized 
	login com adm - email, senha, perfil (vazios ou nulo) - BadRequest
	*/
	readonly string endpoint = "/adm";

	[Fact]
	public async Task TestPost_SemLogin()
	{
		// Given
		var adm = new AdministradorDTO { Email = "criarnovoadm@semautenicacao.com", Senha = "123456", Perfil = Perfil.adm };
		var resp = await client.PostAsJsonAsync(endpoint, adm);

		Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
	}

	[Theory]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task TestPost_Editor(string email, string senha)
	{
		var auth = await Logar(email, senha);

		var criarNovoAdm = new AdministradorDTO { Email = "criarnovoadm@semautenicacao.com", Senha = "123456", Perfil = Perfil.adm };
		var criarNovoEditor = new AdministradorDTO { Email = "criarnovoeditor@semautenicacao.com", Senha = "123456", Perfil = Perfil.editor };

		/* Post sem autenticação */
		var respNovoAdm = await client.PostAsJsonAsync(endpoint, criarNovoAdm);
		var respNovoEditor = await client.PostAsJsonAsync(endpoint, criarNovoEditor);

		Assert.Equal(HttpStatusCode.Unauthorized, respNovoAdm.StatusCode);
		Assert.Equal(HttpStatusCode.Unauthorized, respNovoEditor.StatusCode);

		/* Com autenticação */
		client.DefaultRequestHeaders.Authorization = auth;
		respNovoAdm = await client.PostAsJsonAsync(endpoint, criarNovoAdm);
		respNovoEditor = await client.PostAsJsonAsync(endpoint, criarNovoEditor);

		Assert.Equal(HttpStatusCode.Forbidden, respNovoAdm.StatusCode);
		Assert.Equal(HttpStatusCode.Forbidden, respNovoEditor.StatusCode);
	}

	[Theory]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("adm@teste.com", "1234asdf")]
	public async Task TestPost_ADM(string email, string senha)
	{
		var auth = await Logar(email, senha);

		/* SEM AUTENTICAÇÂO */
		var criarNovoAdm = new AdministradorDTO { Email = "criarnovoadm@semautenicacao.com", Senha = "123456", Perfil = Perfil.adm };
		var criarNovoEditor = new AdministradorDTO { Email = "criarnovoeditor@semautenicacao.com", Senha = "123456", Perfil = Perfil.editor };

		var respNovoAdm = await client.PostAsJsonAsync(endpoint, criarNovoAdm);
		var respNovoEditor = await client.PostAsJsonAsync(endpoint, criarNovoEditor);

		Assert.Equal(HttpStatusCode.Unauthorized, respNovoAdm.StatusCode);
		Assert.Equal(HttpStatusCode.Unauthorized, respNovoEditor.StatusCode);


		/* COM AUTENTICAÇÃO */
		client.DefaultRequestHeaders.Authorization = auth;
		respNovoAdm = await client.PostAsJsonAsync(endpoint, criarNovoAdm);
		respNovoEditor = await client.PostAsJsonAsync(endpoint, criarNovoEditor);

		Assert.Equal(HttpStatusCode.Created, respNovoAdm.StatusCode);
		Assert.Equal(HttpStatusCode.Created, respNovoEditor.StatusCode);

		/* DADOS VAZIOS */
		criarNovoAdm.Email = string.Empty;
		criarNovoEditor.Senha = string.Empty;

		respNovoAdm = await client.PostAsJsonAsync(endpoint, criarNovoAdm);
		respNovoEditor = await client.PostAsJsonAsync(endpoint, criarNovoEditor);

		Assert.Equal(HttpStatusCode.BadRequest, respNovoAdm.StatusCode);
		Assert.Equal(HttpStatusCode.BadRequest, respNovoEditor.StatusCode);
		Assert.Equal(7, db.Administradores.Count()); // 5 iniciais + 2 adicionados
	}

	/* Método parecido com o de TestGetAdministrador - melhor refatorar depois */
	async Task<AuthenticationHeaderValue> Logar(string email, string senha)
	{
		var auth = await ((IUserLogger)this).DoLoginAndReturnAuthHeader(client, email, senha);
		return auth;
	}
}
