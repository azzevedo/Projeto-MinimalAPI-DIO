using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enum;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;



var builder = WebApplication.CreateBuilder(args);

#region JWT
var key = builder.Configuration.GetSection("Jwt").GetSection("Key").ToString();

builder.Services.AddAuthentication(option =>
{
	option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateLifetime = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? "KeyÉNull")),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});

builder.Services.AddAuthorization();

// Adicionar .RequireAuthorization nas rotas que precisam de autorização
#endregion

#region EndPointSwagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
	options =>
	{
		options.AddSecurityDefinition(
			"bearer",
			new OpenApiSecurityScheme()
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Insira o token JWT"
			}
		);

		options.AddSecurityRequirement(
			new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "bearer"
						}
					},
					Array.Empty<string>()
				}
			}
		);
	}
);
#endregion

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

var environment = builder.Environment.EnvironmentName;
if (environment == "Testing")
{
	builder.Services.AddDbContext<DbContexto>(
		options =>
		{
			options.UseSqlite("DataSource=:memory:");
		}
	);
}
else
{
	// Configurar DbContexto para MySQL
	builder.Services.AddDbContext<DbContexto>(
		options =>
		{
			var sql = builder.Configuration.GetConnectionString("mysql");

			if (string.IsNullOrEmpty(sql))
				throw new InvalidOperationException("String mysql não configurada");

			// Pass the actual connection string value (sql) to UseMySql.
			options.UseMySql(
				sql,
				ServerVersion.AutoDetect(sql)
			);
		}
	);
}



var app = builder.Build();

#region SWAGGER
if (app.Environment.IsDevelopment())
{
	// app.mapopen
	app.UseSwagger();
	app.UseSwaggerUI(
		options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API");
			options.RoutePrefix = string.Empty;
		}
	);
}
#endregion

#region ADM
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

app.MapPost("/adm/login",
	([FromBody] LoginDTO loginDTO, IAdministradorServico servico) =>
	{
		if (servico.IsValidLogin(loginDTO, out Administrador administrador))
		{
			// return Results.Ok($"{administrador?.Email}");
			string token = GenerateTokenJWT(administrador);
			var admLogado = new AdmLogado()
			{
				Email = administrador.Email,
				Perfil = administrador.Perfil,
				Token = token
			};
			return Results.Accepted($"Login com sucesso!", admLogado);
		}
		else
		{
			return Results.BadRequest($"Não autorizado");
		}
	}
).AllowAnonymous().WithTags("ADM");

app.MapPost("/adm/",
	async ([FromBody] AdministradorDTO admDTO, IAdministradorServico servico) =>
	{
		ValidationError validacao = new();
		if (string.IsNullOrEmpty(admDTO.Email))
			validacao.Messages.Add($"Email inválido");
		if (string.IsNullOrEmpty(admDTO.Senha))
			validacao.Messages.Add($"Senha inválida");
		if (string.IsNullOrEmpty(admDTO.Perfil.ToString()))
			validacao.Messages.Add($"Perfil inválido");

		if (validacao.Messages.Count > 0)
			return Results.BadRequest(validacao);

		Administrador adm = new()
		{
			Email = admDTO.Email,
			Senha = admDTO.Senha,
			Perfil = admDTO.Perfil.ToString()
		};

		await servico.Incluir(adm);

		return Results.Created("Novo perfil adm criado", adm);
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("ADM");

app.MapGet("/admins",
	async (IAdministradorServico servico, [FromQuery] int pagina = 1) =>
	{
		var result = await servico.GetAdministradores(pagina);

		var administradoresMV = new List<AdministradorModelView>();

		foreach (var adm in result)
		{
			administradoresMV.Add(
				new()
				{
					Email = adm.Email,
					Perfil = adm.Perfil
				}
			);
		}

		return administradoresMV;
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("ADM");


app.MapGet("/admins/{id}",
	async ([FromRoute] int id, IAdministradorServico servico) =>
	{
		var result = await servico.GetAdministradorByIdAsync(id);

		if (result is not null)
		{
			AdministradorModelView adm = new()
			{
				Email = result.Email,
				Perfil = result.Perfil
			};
			return Results.Ok(adm);
		}
		
		return Results.NotFound($"ID [{id}] não encontrado!");
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("ADM");
#endregion


#region Veiculos
app.MapPost("/veiculos",
	async ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico servico) =>
	{
		var validacao = ValidateDTO(veiculoDTO);
		if (validacao.Messages.Count > 0)
		{
			return Results.BadRequest(validacao);
		}

		Veiculo v = new()
		{
			Nome = veiculoDTO.Nome,
			Marca = veiculoDTO.Marca,
			Ano = veiculoDTO.Ano
		};
		IResult resultado = await servico.InsertVeiculo(v);
		return Results.Created("Criado", v);
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("VEICULOS");

app.MapGet("/veiculos",
	async (IVeiculoServico servico, [FromQuery] int pagina = 1) =>
	{
		var result = await servico.GetVeiculos(pagina);
		return Results.Ok(result);
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm},{Perfil.editor}" })
.WithTags("VEICULOS");

app.MapGet("/veiculos/{id}",
	async ([FromRoute] int id, IVeiculoServico servico) =>
	{
		var result = await servico.GetVeiculoById(id);

		if (result is not null)
			return Results.Ok(result);
		
		return Results.NotFound($"ID [{id}] não encontrado!");
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm},{Perfil.editor}" })
.WithTags("VEICULOS");

app.MapPut("/veiculos/{id}",
	async ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico servico) =>
	{
		Veiculo? v = await servico.GetVeiculoById(id);

		if (v is null)
			return Results.NotFound($"ID [{id}] não encontrado!\nImpossível atualizar");

		var validacao = ValidateDTO(veiculoDTO);
		if (validacao.Messages.Count > 0)
			return Results.BadRequest(validacao);
		

		v.Nome = veiculoDTO.Nome;
		v.Marca = veiculoDTO.Marca;
		v.Ano = veiculoDTO.Ano;

		var result = await servico.UpdateVeiculo(v);
		return Results.Created("Alterado com sucesso", v);
		// var result = await servico.UpdateVeiculo(veiculo);
		// return result;
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("VEICULOS");

app.MapDelete("/veiculos/{id}",
	async ([FromRoute] int id, IVeiculoServico servico) =>
	{
		Veiculo? v = await servico.GetVeiculoById(id);
		if (v is null)
		{
			return Results.NotFound($"O ID [{id} não existe]");
		}

		await servico.DeleteVeiculo(v);
		return Results.Ok($"{v.Id} == {v.Nome} deletado!");
	}
)
.RequireAuthorization(new AuthorizeAttribute() { Roles = $"{Perfil.adm}" })
.WithTags("VEICULOS");
#endregion

#region HELPERS
static ValidationError ValidateDTO(VeiculoDTO veiculoDTO)
{
	ValidationError validacao = new();

	if (string.IsNullOrEmpty(veiculoDTO.Nome))
	{
		validacao.Messages.Add($"Informe o nome");
	}
	if (string.IsNullOrEmpty(veiculoDTO.Marca))
	{
		validacao.Messages.Add($"Informe a marca");
	}
	if (veiculoDTO.Ano < 1960)
	{
		validacao.Messages.Add($"Somente veículo a partir de 1960");
	}

	return validacao;
}

string GenerateTokenJWT(Administrador adm)
{
	/*
	Este token junta os claims, data de expiração, e a credencial (configurada com a chave de segurança).
	*/
	var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? "KeyÉNull"));
	var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
	var claims = new List<Claim>()
    {
        new(ClaimTypes.Email, adm.Email),
		new(ClaimTypes.Role, adm.Perfil)
    };
	var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credential);

	return new JwtSecurityTokenHandler().WriteToken(token);
}
#endregion


#region JWT use
// Esta deve ser a ordem (authentication primeiro)
// E AddAuthentication lá em cima do arquivo
app.UseAuthentication();
app.UseAuthorization();
#endregion

app.Run();
