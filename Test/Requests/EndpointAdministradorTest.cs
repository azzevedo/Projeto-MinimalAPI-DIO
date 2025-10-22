using System.Net.Http.Json;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Enum;

namespace Test.Requests;

public class EndpointAdministradorTest
{
	private CustomWebApplicationFactory _factory = default!;
	private HttpClient _client = default!;

	public EndpointAdministradorTest()
	{
		_factory = new CustomWebApplicationFactory();
		_client = _factory.CreateClient();
	}

	public void Cleanup()
	{
		_client.Dispose();
		_factory.Dispose();
	}



	public async Task TestCriarAdministradorEFazerLogin()
	{
		// Arrange
		var novoAdm = new AdministradorDTO
		{
			Email = "",
			Senha = "admin123",
			Perfil = Perfil.adm
		};

		var response = await _client.PostAsJsonAsync("/adm/", novoAdm);
		Assert.IsTrue(response.IsSuccessStatusCode, "Falha ao criar novo administrador");
	}

	public async Task Get_Administradores_ReturnsSuccessStatusCode()
	{
		// Arrange
		var request = "/administradores";

		// Act
		var response = await _client.GetAsync(request);

		// Assert
		response.EnsureSuccessStatusCode(); // Status Code 200-299
	}
}
