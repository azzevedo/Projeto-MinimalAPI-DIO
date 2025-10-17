using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minimal_api.Dominio.Entidades;


public class Veiculo
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; } = default!;

	[Required]
	[StringLength(20)]
	public string Nome { get; set; } = default!;

	[Required]
	[StringLength(20)]
	public string Marca { get; set; } = default!;

	public int Ano { get; set; } = default!;
}
