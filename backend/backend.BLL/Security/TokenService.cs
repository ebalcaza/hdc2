using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.BLL.DTOs;
using backend.DAL.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend.BLL.Security;

public interface ITokenService
{
    AuthResponse Crear(Usuario usuario);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public AuthResponse Crear(Usuario usuario)
    {
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Clave));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        // Claims con nombres cortos: la API los lee con MapInboundClaims = false
        var claims = new[]
        {
            new Claim("sub", usuario.IdUsuario.ToString()),
            new Claim("email", usuario.Email),
            new Claim("role", usuario.Rol.Nombre)
        };

        var expira = DateTime.UtcNow.AddMinutes(_settings.ExpiraMinutos);
        var token = new JwtSecurityToken(
            issuer: _settings.Emisor,
            audience: _settings.Audiencia,
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthResponse(jwt, expira, usuario.IdUsuario, usuario.Nombre, usuario.Rol.Nombre);
    }
}
