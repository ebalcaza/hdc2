using backend.BLL.Exceptions;
using backend.BLL.Security;
using backend.DAL.Entities;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IAdministradorInicialService
{
    /// <summary>Crea el administrador si todavía no existe una cuenta con ese email.</summary>
    Task AsegurarAsync(string email, string contrasena, CancellationToken ct = default);
}

public class AdministradorInicialService : IAdministradorInicialService
{
    private readonly IUsuarioRepository _usuarios;
    public AdministradorInicialService(IUsuarioRepository usuarios) => _usuarios = usuarios;

    public async Task AsegurarAsync(string email, string contrasena, CancellationToken ct = default)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        if (await _usuarios.ExisteEmailAsync(emailNormalizado, ct)) return;

        if (contrasena.Length < 8)
            throw new BusinessRuleException("La contraseña del administrador inicial debe tener al menos 8 caracteres.");

        var rol = await _usuarios.ObtenerRolPorNombreAsync(Roles.Administrador, ct)
            ?? throw new BusinessRuleException($"El rol '{Roles.Administrador}' no está cargado en la tabla ROL.");

        var ahora = DateTime.UtcNow;
        await _usuarios.AgregarAsync(new Usuario
        {
            IdRol = rol.IdRol,
            Email = emailNormalizado,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(contrasena),
            Nombre = "Administrador",
            FechaAlta = ahora,
            FechaUltimaModificacion = ahora
        }, ct);

        await _usuarios.GuardarCambiosAsync(ct);
    }
}
