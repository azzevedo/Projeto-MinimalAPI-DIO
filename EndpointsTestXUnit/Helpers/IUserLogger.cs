using System.Net.Http.Headers;
using System.Net.Http.Json;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.ModelViews;

namespace EndpointsTestXUnit.Helpers;

/// <summary>
/// Interface utilizada para logar com Adm ou Editor
/// </summary>
public interface IUserLogger
{
	/// <summary>
	/// Faz login e retorna um AuthenticationHeaderValue <br/>
	/// Útil para os testes de endpoints <br/>
	/// PS.: aproveitando a funcionalidade de "corpo" nos métodos das interfaces
	/// </summary>
	/// <param name="client"></param>
	/// <param name="email"></param>
	/// <param name="senha"></param>
	/// <returns>AuthenticationHeaderValue</returns>
	/// <exception cref="InvalidOperationException"></exception>
	private static async Task<AuthenticationHeaderValue> DoLoginAndReturnAuthHeader(HttpClient client, string email, string senha)
	{
		var dto = new AdministradorDTO { Email = email, Senha = senha };
		var loginResponse = await client.PostAsJsonAsync("/adm/login", dto);

		loginResponse.EnsureSuccessStatusCode();

		var admLogado = await loginResponse.Content.ReadFromJsonAsync<AdmLogado>()
			?? throw new InvalidOperationException("Sem token");

		return new AuthenticationHeaderValue("Bearer", admLogado.Token);
	}

	/* Diminue (DRY) os códigos nas classes de testes */
	public static async Task Autenticar(HttpClient client, string email, string senha)
	{
		client.DefaultRequestHeaders.Authorization = await DoLoginAndReturnAuthHeader(client, email, senha);
	}
}
