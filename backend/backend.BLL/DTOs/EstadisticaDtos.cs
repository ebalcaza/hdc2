using System.ComponentModel.DataAnnotations;

namespace backend.BLL.DTOs;

/// <summary>Filtros (todos opcionales) con los que el administrador arma su reporte estadístico.</summary>
public class FiltroEstadisticaRequest
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }

    /// <summary>"Individuo" u "Organizacion".</summary>
    public string? TipoUsuario { get; set; }

    public long? IdFuente { get; set; }
    public long? IdAlcance { get; set; }
    public long? IdTipoEmision { get; set; }

    [MaxLength(100)] public string? Provincia { get; set; }
    [MaxLength(200)] public string? Ciudad { get; set; }
}

public record ItemEstadisticaDto(string Etiqueta, decimal TotalCo2Kg, int CantidadRegistros, decimal Porcentaje);

public record EstadisticaDto(
    DateTime GeneradoEn,
    FiltroEstadisticaRequest Filtros,
    int CantidadReportes,
    int CantidadRegistros,
    decimal TotalCo2Kg,
    List<ItemEstadisticaDto> PorFuente,
    List<ItemEstadisticaDto> PorAlcance,
    List<ItemEstadisticaDto> PorTipoEmision,
    List<ItemEstadisticaDto> PorProvincia,
    List<ItemEstadisticaDto> PorTipoUsuario,
    List<ItemEstadisticaDto> PorMes);

public record ItemCatalogoDto(long Id, string Nombre);

public record ArchivoDto(byte[] Contenido, string NombreArchivo, string TipoContenido);
