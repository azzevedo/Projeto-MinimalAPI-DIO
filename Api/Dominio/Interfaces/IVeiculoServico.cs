using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces;


public interface IVeiculoServico
{
	Task<List<Veiculo>> GetVeiculos(int pagina = 1, string? nome = null, string? marca = null);
	// ActionResult<List<Veiculo>>
	Task<Veiculo?> GetVeiculoById(int id);
	Task<IResult> InsertVeiculo(Veiculo veiculo);
	Task<Veiculo> UpdateVeiculo(Veiculo veiculo);
	Task DeleteVeiculo(Veiculo veiculo);
}