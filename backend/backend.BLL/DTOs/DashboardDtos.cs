namespace backend.BLL.DTOs;

/// <summary>Un mes de la curva de evolución. Mes tiene el formato "2026-10".</summary>
public record PuntoMensualDto(string Mes, decimal TotalCo2Kg, decimal AcumuladoCo2Kg);

/// <summary>Resumen de las emisiones del usuario logueado (todos sus reportes activos).</summary>
public record DashboardDto(
    DateTime GeneradoEn,
    decimal TotalCo2Kg,
    int CantidadReportes,
    int CantidadRegistros,
    decimal PromedioPorReporteKg,
    DateTime? UltimoReporteFecha,
    decimal? UltimoReporteCo2Kg,
    decimal EsteMesCo2Kg,
    decimal MesAnteriorCo2Kg,
    decimal? VariacionMensualPorcentaje,
    List<ItemEstadisticaDto> PorAlcance,
    List<ItemEstadisticaDto> PorTipoEmision,
    List<ItemEstadisticaDto> PorFuente,
    List<PuntoMensualDto> SerieMensual);
