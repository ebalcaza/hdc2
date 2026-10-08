using System.ComponentModel.DataAnnotations;

namespace backend.BLL.DTOs;

// ---------------- Cuentas de usuarios ----------------

public class FiltroUsuariosRequest
{
    /// <summary>Busca en email, nombre, apellido, DNI, razón social y CUIT.</summary>
    [MaxLength(100)] public string? Texto { get; set; }

    /// <summary>"Individuo", "Organizacion" o "Administrador".</summary>
    public string? TipoUsuario { get; set; }

    /// <summary>true = cuentas activas, false = dadas de baja, vacío = todas.</summary>
    public bool? Activo { get; set; }

    [Range(1, int.MaxValue)] public int Pagina { get; set; } = 1;
    [Range(1, 100)] public int Tamano { get; set; } = 20;
}

public record PaginaDto<T>(List<T> Items, int Total, int Pagina, int Tamano);

public record UsuarioResumenDto(
    long IdUsuario,
    string Email,
    string Nombre,
    string TipoUsuario,
    bool Activo,
    DateTime FechaAlta,
    DateTime? FechaBaja,
    int CantidadReportes);

public record UsuarioDetalleDto(
    long IdUsuario,
    string Email,
    string Nombre,
    string TipoUsuario,
    bool Activo,
    DateTime FechaAlta,
    DateTime? FechaBaja,
    int CantidadReportes,
    string? Apellido,
    string? Telefono,
    DateTime? FechaNacimiento,
    string? Dni,
    string? Genero,
    string? RazonSocial,
    string? Cuit,
    string? Area,
    string? Descripcion,
    int? CantidadMiembros,
    List<DomicilioDto> Domicilios);

public class CrearAdministradorRequest
{
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Contrasena { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Apellido { get; set; }
}

// ---------------- Factores de emisión ----------------

public class FuenteEmisionRequest
{
    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, long.MaxValue, ErrorMessage = "Elegí una unidad de medida.")]
    public long IdUnidad { get; set; }

    [Range(1, long.MaxValue, ErrorMessage = "Elegí un tipo de emisión.")]
    public long IdTipoEmision { get; set; }

    public long? IdAlcance { get; set; }

    /// <summary>kg de CO2 por unidad de consumo.</summary>
    [Range(typeof(decimal), "0.000001", "999999999", ErrorMessage = "El valor del factor debe ser mayor a 0.")]
    public decimal ValorFactorEmision { get; set; }
}

public record FuenteEmisionAdminDto(
    long IdFuente,
    string Nombre,
    long IdUnidad,
    string Unidad,
    long? IdAlcance,
    string? Alcance,
    long IdTipoEmision,
    string TipoEmision,
    decimal ValorFactorEmision,
    bool EnUso);
