using backend.API.Extensions;
using backend.BLL.DTOs;
using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

/// <summary>Panel principal de individuos y organizaciones: sus propias emisiones.</summary>
[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = Roles.Individuo + "," + Roles.Organizacion)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;
    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Obtener(CancellationToken ct) =>
        Ok(await _dashboard.ObtenerAsync(User.ObtenerIdUsuario(), ct));
}
