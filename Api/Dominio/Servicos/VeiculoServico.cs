using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;



namespace minimal_api.Dominio.Servicos;

public class VeiculoServico(DbContexto db) : IVeiculoServico
{
	readonly DbContexto _db = db;

	public async Task DeleteVeiculo(Veiculo veiculo)
	{
		_db.Veiculos.Remove(veiculo);
		await _db.SaveChangesAsync();
	}

	public async Task<Veiculo?> GetVeiculoById(int id)
	{
		Veiculo? v = await _db.Veiculos.FindAsync(id);
		return v;
	}

	public async Task<IResult> GetVeiculos(int pagina, string? nome = null, string? marca = null)
	{
		int itensPorPagina = 10;

		var veiculos = _db.Veiculos.AsQueryable();
		// List<Veiculo> veiculos;
		if (nome is not null)
			veiculos = veiculos.Where(v => v.Nome.Contains(nome, StringComparison.CurrentCultureIgnoreCase));

		veiculos = veiculos.Skip(itensPorPagina * (pagina - 1)).Take(itensPorPagina);

		var v = await veiculos.ToListAsync();
		return Results.Ok(v);
	}

	public async Task<IResult> InsertVeiculo(Veiculo veiculo)
	{
		// var existente = await _db.Veiculos.FindAsync(veiculo);

		// if (existente is not null)
        // {
		// 	return Results.Conflict($"O veículo {veiculo.Nome} já existe");
        // }

		_db.Veiculos.Add(veiculo);
		await _db.SaveChangesAsync();
		return Results.Created("Informação inserida!", veiculo);
	}

	public async Task<Veiculo> UpdateVeiculo(Veiculo veiculo)
	{
		_db.Veiculos.Update(veiculo);
		await _db.SaveChangesAsync();
		return veiculo;
	}
}
