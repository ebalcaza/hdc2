using backend.BLL.DTOs;
using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>Alta y mantenimiento de fuentes y factores de emisión (solo administradores).</summary>
[ApiController]
[Route("api/admin/fuentes-emision")]
[Authorize(Roles = Roles.Administrador)]
public class AdminFuentesEmisionController : ControllerBase
{
    private readonly IAdminFuenteService _fuentes;
    public AdminFuentesEmisionController(IAdminFuenteService fuentes) => _fuentes = fuentes;

    [HttpGet]
    public async Task<ActionResult<List<FuenteEmisionAdminDto>>> Listar(CancellationToken ct) =>
        Ok(await _fuentes.ListarAsync(ct));

    [HttpPost]
    public async Task<ActionResult<FuenteEmisionAdminDto>> Crear([FromBody] FuenteEmisionRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await _fuentes.CrearAsync(request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<FuenteEmisionAdminDto>> Actualizar(
        long id, [FromBody] FuenteEmisionRequest request, CancellationToken ct) =>
        Ok(await _fuentes.ActualizarAsync(id, request, ct));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id, CancellationToken ct)
    {
        await _fuentes.EliminarAsync(id, ct);
        return NoContent();
    }
}
