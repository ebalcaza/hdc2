using backend.API.Extensions;
using backend.BLL.DTOs;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>Fuentes de emisión disponibles para armar un reporte.</summary>
[ApiController]
[Authorize]
[Route("api/fuentes-emision")]
public class FuentesEmisionController : ControllerBase
{
    private readonly IFuenteEmisionService _fuentes;
    public FuentesEmisionController(IFuenteEmisionService fuentes) => _fuentes = fuentes;

    [HttpGet]
    public async Task<ActionResult<List<FuenteEmisionDto>>> Listar(CancellationToken ct) =>
        Ok(await _fuentes.ListarAsync(ct));
}

/// <summary>Domicilios del usuario logueado (cada registro de consumo se asocia a uno).</summary>
[ApiController]
[Authorize]
[Route("api/domicilios")]
public class DomiciliosController : ControllerBase
{
    private readonly IDomicilioService _domicilios;
    public DomiciliosController(IDomicilioService domicilios) => _domicilios = domicilios;

    [HttpGet]
    public async Task<ActionResult<List<DomicilioDto>>> Listar(CancellationToken ct) =>
        Ok(await _domicilios.ListarAsync(User.ObtenerIdUsuario(), ct));

    [HttpPost]
    public async Task<ActionResult<DomicilioDto>> Crear([FromBody] DomicilioRequest request, CancellationToken ct) =>
        Ok(await _domicilios.CrearAsync(User.ObtenerIdUsuario(), request, ct));
}
