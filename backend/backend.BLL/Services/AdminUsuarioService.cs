using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.BLL.Mapeos;
using backend.BLL.Security;
using backend.DAL.Entities;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IAdminUsuarioService
{
    Task<PaginaDto<UsuarioResumenDto>> ListarAsync(FiltroUsuariosRequest request, CancellationToken ct = default);
    Task<UsuarioDetalleDto> ObtenerAsync(long idUsuario, CancellationToken ct = default);
    Task<UsuarioDetalleDto> DarDeBajaAsync(long idAdmin, long idUsuario, CancellationToken ct = default);
    Task<UsuarioDetalleDto> DarDeAltaAsync(long idAdmin, long idUsuario, CancellationToken ct = default);
    Task<UsuarioResumenDto> CrearAdministradorAsync(long idAdmin, CrearAdministradorRequest request, CancellationToken ct = default);
}

/// <summary>El administrador gestiona las cuentas de los usuarios.</summary>
public class AdminUsuarioService : IAdminUsuarioService
{
    private readonly IAdminUsuarioRepository _admin;
    private readonly IUsuarioRepository _usuarios;

    public AdminUsuarioService(IAdminUsuarioRepository admin, IUsuarioRepository usuarios)
    {
        _admin = admin;
        _usuarios = usuarios;
    }

    public async Task<PaginaDto<UsuarioResumenDto>> ListarAsync(FiltroUsuariosRequest request, CancellationToken ct = default)
    {
        var tipo = string.IsNullOrWhiteSpace(request.TipoUsuario) ? null : request.TipoUsuario;
        if (tipo is not null && tipo != Roles.Individuo && tipo != Roles.Organizacion && tipo != Roles.Administrador)
            throw new BusinessRuleException("El tipo de usuario debe ser 'Individuo', 'Organizacion' o 'Administrador'.");

        var filtro = new FiltroUsuarios(request.Texto, tipo, request.Activo);
        var (usuarios, total) = await _admin.BuscarAsync(filtro, request.Pagina, request.Tamano, ct);

        var reportes = await _admin.ContarReportesActivosAsync(usuarios.Select(u => u.IdUsuario).ToList(), ct);
        var items = usuarios
            .Select(u => UsuarioMapper.AResumen(u, reportes.TryGetValue(u.IdUsuario, out var c) ? c : 0))
            .ToList();

        return new PaginaDto<UsuarioResumenDto>(items, total, request.Pagina, request.Tamano);
    }

    public async Task<UsuarioDetalleDto> ObtenerAsync(long idUsuario, CancellationToken ct = default)
    {
        var usuario = await _admin.ObtenerAsync(idUsuario, ct)
            ?? throw new NotFoundException("El usuario no existe.");

        var reportes = await _admin.ContarReportesActivosAsync(new List<long> { idUsuario }, ct);
        return UsuarioMapper.ADetalle(usuario, reportes.TryGetValue(idUsuario, out var c) ? c : 0);
    }

    public async Task<UsuarioDetalleDto> DarDeBajaAsync(long idAdmin, long idUsuario, CancellationToken ct = default)
    {
        if (idAdmin == idUsuario)
            throw new BusinessRuleException("No podés dar de baja tu propia cuenta.");

        var usuario = await _admin.ObtenerAsync(idUsuario, ct)
            ?? throw new NotFoundException("El usuario no existe.");

        if (usuario.FechaBaja is not null)
            throw new BusinessRuleException("La cuenta ya está dada de baja.");

        var ahora = DateTime.UtcNow;
        usuario.FechaBaja = ahora;
        usuario.AutorUltimaModificacion = idAdmin;
        usuario.FechaUltimaModificacion = ahora;
        await _admin.GuardarCambiosAsync(ct);

        return await ObtenerAsync(idUsuario, ct);
    }

    public async Task<UsuarioDetalleDto> DarDeAltaAsync(long idAdmin, long idUsuario, CancellationToken ct = default)
    {
        var usuario = await _admin.ObtenerAsync(idUsuario, ct)
            ?? throw new NotFoundException("El usuario no existe.");

        if (usuario.FechaBaja is null)
            throw new BusinessRuleException("La cuenta ya está activa.");

        usuario.FechaBaja = null;
        usuario.AutorUltimaModificacion = idAdmin;
        usuario.FechaUltimaModificacion = DateTime.UtcNow;
        await _admin.GuardarCambiosAsync(ct);

        return await ObtenerAsync(idUsuario, ct);
    }

    public async Task<UsuarioResumenDto> CrearAdministradorAsync(
        long idAdmin, CrearAdministradorRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await _usuarios.ExisteEmailAsync(email, ct))
            throw new BusinessRuleException("Ya existe una cuenta con ese email.");

        var rol = await _usuarios.ObtenerRolPorNombreAsync(Roles.Administrador, ct)
            ?? throw new BusinessRuleException($"El rol '{Roles.Administrador}' no está cargado en la tabla ROL.");

        var ahora = DateTime.UtcNow;
        var usuario = new Usuario
        {
            IdRol = rol.IdRol,
            Rol = rol,
            Email = email,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            Nombre = request.Nombre.Trim(),
            Apellido = string.IsNullOrWhiteSpace(request.Apellido) ? null : request.Apellido.Trim(),
            FechaAlta = ahora,
            AutorUltimaModificacion = idAdmin,
            FechaUltimaModificacion = ahora
        };

        await _usuarios.AgregarAsync(usuario, ct);
        await _usuarios.GuardarCambiosAsync(ct);

        return UsuarioMapper.AResumen(usuario, 0);
    }
}
