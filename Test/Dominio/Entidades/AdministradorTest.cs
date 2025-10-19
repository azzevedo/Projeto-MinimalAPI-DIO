using minimal_api.Dominio.Entidades;

namespace Test.Dominio.Entidades;

[TestClass]
public class AdministradorTest
{
	[TestMethod]
	public void TestGetSetPropriedades()
	{
		// Arrange -> criação de variáveis

		Administrador adm = new()
		{
			// SET
			Id = 1,
			Email = "zze@teste.com",
			Senha = "asdfqwer1234",
			Perfil = "adm"
		};
		// Act -> ações a executar


		// Assert -> validação dos dados - GET

		Assert.AreEqual(1, adm.Id);
		Assert.AreEqual("zze@teste.com", adm.Email);
		Assert.AreEqual("asdfqwer1234", adm.Senha);
		Assert.AreEqual("adm", adm.Perfil);
	}
}
