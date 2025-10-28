using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;


public interface IAdministradorServico
{
	// List<Administrador> Login(LoginDTO loginDTO);
	// bool Login(LoginDTO loginDTO);
	// Administrador? Login(LoginDTO loginDTO);
	bool IsValidLogin(LoginDTO loginDTO, out Administrador administrador);

	Task Incluir(Administrador adm);
	Task<List<Administrador>> GetAdministradores(int pagina);
	Task<Administrador?> GetAdministradorByIdAsync(int id);
	Task DeleteAdministrador(Administrador adm);
	Task<Administrador> UpdateAdministrador(Administrador adm);
}