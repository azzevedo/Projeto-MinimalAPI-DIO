using System.Net;
using System.Net.Http.Json;
using EndpointsTestXUnit.Helpers;
using minimal_api.Dominio.DTO;

namespace EndpointsTestXUnit.Endpoints.Administrador;

public class TestLogin : ApiTestBase
{
#region Sucesso
	[Theory]
	[InlineData("adm@teste.com", "1234asdf")]
	[InlineData("mamonas@assassinas.com", "1406")]
	[InlineData("mundo@animal.com", "dornascostas")]
	[InlineData("vira@vira.com", "monoteta")]
	[InlineData("lavem@alemao.com", "facade2legumes")]
	public async Task Test_Login(string email, string senha)
	{
		var adm = new AdministradorDTO { Email = email, Senha = senha };

		var response = await client.PostAsJsonAsync("adm/login", adm);
		var json = await response.Content.ReadAsStringAsync();

		Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
		Assert.Contains("token", json);
	}
#endregion

#region Falha
	[Theory]
	[InlineData("mamonas@assassinas1234.com", "1406")]
	[InlineData("adm@TTTeste.com", "1234asdf")]
	[InlineData("vira@vira.com", "asdf")]
	[InlineData("mundo@animal.com", "1234zxcv")]
	public async Task Test_Login_Falha(string email, string senha)
	{
		var adm = new AdministradorDTO { Email = email, Senha = senha };

		var response = await client.PostAsJsonAsync("adm/login", adm);
		var json = await response.Content.ReadAsStringAsync();

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		Assert.DoesNotContain("token", json);
	}
#endregion

}
