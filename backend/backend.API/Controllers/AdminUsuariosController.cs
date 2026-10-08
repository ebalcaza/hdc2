using backend.API.Extensions;
using backend.BLL.DTOs;
using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>Gestión de las cuentas de usuarios (solo administradores).</summary>
[ApiController]
[Route("api/admin/usuarios")]
[Authorize(Roles = Roles.Administrador)]
public class AdminUsuariosController : ControllerBase
{
    private readonly IAdminUsuarioService _usuarios;
    public AdminUsuariosController(IAdminUsuarioService usuarios) => _usuarios = usuarios;

    // Ejemplo: GET /api/admin/usuarios?texto=perez&tipoUsuario=Individuo&activo=true&pagina=1&tamano=20
    [HttpGet]
    public async Task<ActionResult<PaginaDto<UsuarioResumenDto>>> Listar([FromQuery] FiltroUsuariosRequest filtros, CancellationToken ct) =>
        Ok(await _usuarios.ListarAsync(filtros, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UsuarioDetalleDto>> Obtener(long id, CancellationToken ct) =>
        Ok(await _usuarios.ObtenerAsync(id, ct));

    [HttpPost("{id:long}/baja")]
    public async Task<ActionResult<UsuarioDetalleDto>> DarDeBaja(long id, CancellationToken ct) =>
        Ok(await _usuarios.DarDeBajaAsync(User.ObtenerIdUsuario(), id, ct));

    [HttpPost("{id:long}/alta")]
    public async Task<ActionResult<UsuarioDetalleDto>> DarDeAlta(long id, CancellationToken ct) =>
        Ok(await _usuarios.DarDeAltaAsync(User.ObtenerIdUsuario(), id, ct));

    [HttpPost("administradores")]
    public async Task<ActionResult<UsuarioResumenDto>> CrearAdministrador(
        [FromBody] CrearAdministradorRequest request, CancellationToken ct)
    {
        var usuario = await _usuarios.CrearAdministradorAsync(User.ObtenerIdUsuario(), request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = usuario.IdUsuario }, usuario);
    }
}
