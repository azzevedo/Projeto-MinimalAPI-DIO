using System.Net;
using EndpointsTestXUnit.Helpers;

namespace EndpointsTestXUnit.Endpoints.Administrador;

public class HomeTest : ApiTestBase
{
	[Fact]
	public async Task Test_HomeEndpoint_Sucesso()
	{
		// Arrange - Act
		var response = await client.GetAsync("/");
		string content = await response.Content.ReadAsStringAsync();

		// Assert
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		Assert.Contains("Bem vindo à API de veículos".ToLower(), content.ToLower());
	}
}
