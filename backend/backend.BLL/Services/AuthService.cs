using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.BLL.Mapeos;
using backend.BLL.Security;
using backend.DAL.Entities;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegistrarIndividuoAsync(RegistroIndividuoRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegistrarOrganizacionAsync(RegistroOrganizacionRequest request, CancellationToken ct = default);
}

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly ITokenService _tokens;

    public AuthService(IUsuarioRepository usuarios, ITokenService tokens)
    {
        _usuarios = usuarios;
        _tokens = tokens;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorEmailAsync(NormalizarEmail(request.Email), ct);

        // Mismo mensaje para email inexistente, usuario dado de baja o clave incorrecta
        if (usuario is null
            || usuario.FechaBaja is not null
            || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena))
        {
            throw new UnauthorizedBusinessException("Email o contraseña incorrectos.");
        }

        return _tokens.Crear(usuario);
    }

    public async Task<AuthResponse> RegistrarIndividuoAsync(RegistroIndividuoRequest r, CancellationToken ct = default)
    {
        var usuario = await CrearUsuarioBaseAsync(
            Roles.Individuo, r.Email, r.Contrasena, r.Nombre, r.Apellido, r.FechaNacimiento, r.Domicilio, ct);

        usuario.Individuo = new Individuo { Dni = r.Dni, Genero = r.Genero?.Trim() };

        return await GuardarYAutenticarAsync(usuario, ct);
    }

    public async Task<AuthResponse> RegistrarOrganizacionAsync(RegistroOrganizacionRequest r, CancellationToken ct = default)
    {
        var razon = r.RazonSocial.Trim();
        var nombre = razon.Length > 100 ? razon[..100] : razon;   // USUARIOS.NOMBRE admite 100

        var usuario = await CrearUsuarioBaseAsync(
            Roles.Organizacion, r.Email, r.Contrasena, nombre, null, null, r.Domicilio, ct);

        usuario.Organizacion = new Organizacion
        {
            RazonSocial = razon,
            Cuit = r.Cuit.Trim(),
            Area = r.Area.Trim(),
            Descripcion = r.Descripcion?.Trim(),
            CantidadMiembros = r.CantidadMiembros
        };

        return await GuardarYAutenticarAsync(usuario, ct);
    }

    private async Task<Usuario> CrearUsuarioBaseAsync(
        string nombreRol, string email, string contrasena, string nombre, string? apellido,
        DateTime? fechaNacimiento, DomicilioRequest domicilio, CancellationToken ct)
    {
        var emailNormalizado = NormalizarEmail(email);

        if (await _usuarios.ExisteEmailAsync(emailNormalizado, ct))
            throw new BusinessRuleException("Ya existe una cuenta con ese email.");

        var rol = await _usuarios.ObtenerRolPorNombreAsync(nombreRol, ct)
            ?? throw new BusinessRuleException($"El rol '{nombreRol}' no está cargado en la tabla ROL.");

        var ahora = DateTime.UtcNow;
        var usuario = new Usuario
        {
            IdRol = rol.IdRol,
            Rol = rol,
            Email = emailNormalizado,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(contrasena),
            Nombre = nombre.Trim(),
            Apellido = apellido?.Trim(),
            FechaNacimiento = fechaNacimiento,
            FechaAlta = ahora,
            FechaUltimaModificacion = ahora
        };

        usuario.Domicilios.Add(DomicilioMapper.ANuevaEntidad(domicilio));
        return usuario;
    }

    private async Task<AuthResponse> GuardarYAutenticarAsync(Usuario usuario, CancellationToken ct)
    {
        await _usuarios.AgregarAsync(usuario, ct);
        await _usuarios.GuardarCambiosAsync(ct);
        return _tokens.Crear(usuario);
    }

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
