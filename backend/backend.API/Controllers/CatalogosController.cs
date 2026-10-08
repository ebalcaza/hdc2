using backend.BLL.DTOs;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>Listas para llenar selectores del front (filtros, formularios).</summary>
[ApiController]
[Authorize]
[Route("api/catalogos")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoService _catalogos;
    public CatalogosController(ICatalogoService catalogos) => _catalogos = catalogos;

    [HttpGet("alcances")]
    public async Task<ActionResult<List<ItemCatalogoDto>>> Alcances(CancellationToken ct) =>
        Ok(await _catalogos.ListarAlcancesAsync(ct));

    [HttpGet("tipos-emision")]
    public async Task<ActionResult<List<ItemCatalogoDto>>> TiposEmision(CancellationToken ct) =>
        Ok(await _catalogos.ListarTiposEmisionAsync(ct));

    [HttpGet("unidades")]
    public async Task<ActionResult<List<ItemCatalogoDto>>> Unidades(CancellationToken ct) =>
        Ok(await _catalogos.ListarUnidadesAsync(ct));
}
