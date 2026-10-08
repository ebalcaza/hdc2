using backend.API.Extensions;
using backend.BLL.DTOs;
using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>"Mis Reportes": solo para individuos y organizaciones (cada uno ve únicamente los suyos).</summary>
[ApiController]
[Route("api/reportes")]
[Authorize(Roles = Roles.Individuo + "," + Roles.Organizacion)]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reportes;
    private readonly IReportePdfService _pdf;

    public ReportesController(IReporteService reportes, IReportePdfService pdf)
    {
        _reportes = reportes;
        _pdf = pdf;
    }

    // RFR05
    [HttpGet]
    public async Task<ActionResult<List<ReporteResumenDto>>> Listar(CancellationToken ct) =>
        Ok(await _reportes.ListarAsync(User.ObtenerIdUsuario(), ct));

    // RFR06
    [HttpGet("{id:long}")]
    public async Task<ActionResult<ReporteDetalleDto>> Obtener(long id, CancellationToken ct) =>
        Ok(await _reportes.ObtenerAsync(User.ObtenerIdUsuario(), id, ct));

    // RFR01
    [HttpPost]
    public async Task<ActionResult<ReporteDetalleDto>> Crear([FromBody] ReporteRequest request, CancellationToken ct)
    {
        var reporte = await _reportes.CrearAsync(User.ObtenerIdUsuario(), request, ct);
        return CreatedAtAction(nameof(Obtener), new { id = reporte.IdReporte }, reporte);
    }

    // RFR03
    [HttpPut("{id:long}")]
    public async Task<ActionResult<ReporteDetalleDto>> Actualizar(
        long id, [FromBody] ReporteRequest request, CancellationToken ct) =>
        Ok(await _reportes.ActualizarAsync(User.ObtenerIdUsuario(), id, request, ct));

    // RFR04
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id, CancellationToken ct)
    {
        await _reportes.EliminarAsync(User.ObtenerIdUsuario(), id, ct);
        return NoContent();
    }

    // RFR07
    [HttpGet("{id:long}/pdf")]
    public async Task<IActionResult> DescargarPdf(long id, CancellationToken ct)
    {
        var archivo = await _pdf.GenerarAsync(User.ObtenerIdUsuario(), id, ct);
        return File(archivo.Contenido, archivo.TipoContenido, archivo.NombreArchivo);
    }
}
