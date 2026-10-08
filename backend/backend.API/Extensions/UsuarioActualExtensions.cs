using System.Security.Claims;

namespace backend.API.Extensions;

public static class UsuarioActualExtensions
{
    /// <summary>Id del usuario logueado, tomado del claim "sub" del JWT.</summary>
    public static long ObtenerIdUsuario(this ClaimsPrincipal user)
    {
        var valor = user.FindFirst("sub")?.Value;
        return long.TryParse(valor, out var id)
            ? id
            : throw new UnauthorizedAccessException("El token no contiene un usuario válido.");
    }
}
