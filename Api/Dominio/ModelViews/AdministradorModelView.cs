using minimal_api.Dominio.Enum;

namespace minimal_api.Dominio.ModelViews;

public record AdministradorModelView
{
	public string Email { get; set; } = default!;
	public string Perfil { get; set; } = default!;
}
