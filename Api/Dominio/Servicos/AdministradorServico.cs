using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;

namespace minimal_api.Dominio.Servicos;

public class AdministradorServico(DbContexto db) : IAdministradorServico
{
	readonly DbContexto _db = db;

	public bool IsValidLogin(LoginDTO loginDTO, out Administrador administrador)
	{
		try
		{
			administrador = _db.Administradores.Where(
				a => a.Email == loginDTO.Email
				&& a.Senha == loginDTO.Senha
			).First();

			return true;
		}
		catch (InvalidOperationException)
		{
			// Adm vazio
			administrador = new();
			return false;
		}
	}

	// public Administrador? Login(LoginDTO loginDTO)
	// {
	// 	var adm = _db.Administradores.Where(
	// 		a => a.Email == loginDTO.Email
	// 		&& a.Senha == loginDTO.Senha
	// 	).First();

	// 	return adm;
	// }

	public async Task Incluir(Administrador adm)
	{
		_db.Administradores.Add(adm);
		await _db.SaveChangesAsync();
	}

	public async Task<List<Administrador>> GetAdministradores(int pagina)
	{
		int itensPorPagina = 10;

		var adms = _db.Administradores.AsQueryable();
		adms = adms.Skip(itensPorPagina * (pagina - 1)).Take(itensPorPagina);

		var admins = await adms.ToListAsync();
		return admins;
	}

	public async Task<Administrador?> GetAdministradorByIdAsync(int id)
	{
		Administrador? adm = await _db.Administradores.FindAsync(id);
		return adm;
	}
}
