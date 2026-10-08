using System.ComponentModel.DataAnnotations;

namespace backend.BLL.DTOs;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;
}

public record AuthResponse(string Token, DateTime ExpiraEn, long IdUsuario, string Nombre, string Rol);

public class RegistroIndividuoRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Contrasena { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Apellido { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 números.")]
    public string Dni { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    [MaxLength(20)]
    public string? Genero { get; set; }

    [Required]
    public DomicilioRequest Domicilio { get; set; } = null!;
}

public class RegistroOrganizacionRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Contrasena { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string RazonSocial { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Cuit { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Area { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    [Range(1, int.MaxValue)]
    public int? CantidadMiembros { get; set; }

    [Required]
    public DomicilioRequest Domicilio { get; set; } = null!;
}
