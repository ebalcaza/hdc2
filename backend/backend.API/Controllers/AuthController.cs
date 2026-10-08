using backend.BLL.DTOs;
using backend.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct) =>
        Ok(await _auth.LoginAsync(request, ct));

    [HttpPost("registro/individuo")]
    public async Task<ActionResult<AuthResponse>> RegistrarIndividuo(
        [FromBody] RegistroIndividuoRequest request, CancellationToken ct) =>
        Ok(await _auth.RegistrarIndividuoAsync(request, ct));

    [HttpPost("registro/organizacion")]
    public async Task<ActionResult<AuthResponse>> RegistrarOrganizacion(
        [FromBody] RegistroOrganizacionRequest request, CancellationToken ct) =>
        Ok(await _auth.RegistrarOrganizacionAsync(request, ct));
}
