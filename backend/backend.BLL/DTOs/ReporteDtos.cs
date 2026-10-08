using System.ComponentModel.DataAnnotations;

namespace backend.BLL.DTOs;

/// <summary>Un consumo: qué fuente, en qué domicilio y cuánto (en la unidad de la fuente).</summary>
public class RegistroRequest
{
    [Range(1, long.MaxValue)]
    public long IdFuente { get; set; }

    [Range(1, long.MaxValue)]
    public long IdDomicilio { get; set; }

    [Range(typeof(decimal), "0.0001", "999999999999")]
    public decimal Valor { get; set; }
}

/// <summary>Sirve para crear un reporte y para reemplazar los datos de consumo de uno existente.</summary>
public class ReporteRequest
{
    [Required, MinLength(1, ErrorMessage = "El reporte necesita al menos un registro de consumo.")]
    public List<RegistroRequest> Registros { get; set; } = new();
}

public record RegistroDto(
    long IdRegistro,
    long IdFuente,
    string Fuente,
    string Unidad,
    string? Alcance,
    string TipoEmision,
    long IdDomicilio,
    string Domicilio,
    decimal Valor,
    decimal FactorEmision,
    decimal Co2Kg);

public record ReporteResumenDto(
    long IdReporte,
    DateTime FechaCreacion,
    DateTime? FechaModificacion,
    int CantidadRegistros,
    decimal TotalCo2Kg);

public record ReporteDetalleDto(
    long IdReporte,
    DateTime FechaCreacion,
    DateTime? FechaModificacion,
    decimal TotalCo2Kg,
    List<RegistroDto> Registros);
