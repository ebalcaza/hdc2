using backend.BLL.DTOs;
using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

[ApiController]
[Route("api/admin/estadisticas")]
[Authorize(Roles = Roles.Administrador)]
public class AdminEstadisticasController : ControllerBase
{
    private readonly IEstadisticaService _estadisticas;
    public AdminEstadisticasController(IEstadisticaService estadisticas) => _estadisticas = estadisticas;

    // RFR02. Ejemplo: GET /api/admin/estadisticas?desde=2026-01-01&hasta=2026-12-31&tipoUsuario=Organizacion&provincia=Buenos Aires
    [HttpGet]
    public async Task<ActionResult<EstadisticaDto>> Generar([FromQuery] FiltroEstadisticaRequest filtros, CancellationToken ct) =>
        Ok(await _estadisticas.GenerarAsync(filtros, ct));
}
