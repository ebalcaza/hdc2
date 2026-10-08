using backend.BLL.DTOs;
using backend.BLL.Services;
using backend.DAL.Entities;

namespace backend.BLL.Mapeos;

internal static class ReporteMapper
{
    public static ReporteResumenDto AResumen(Reporte r) => new(
        r.IdReporte, r.FechaCreacion, r.FechaModificacion, r.Registros.Count, r.TotalCo2Kg);

    public static ReporteDetalleDto ADetalle(Reporte r) => new(
        r.IdReporte, r.FechaCreacion, r.FechaModificacion, r.TotalCo2Kg,
        r.Registros.OrderBy(x => x.IdRegistro).Select(ARegistro).ToList());

    private static RegistroDto ARegistro(Registro x) => new(
        x.IdRegistro,
        x.IdFuente,
        x.Fuente.Nombre,
        x.Fuente.Unidad.Nombre,
        x.Fuente.Alcance?.Nombre,
        x.Fuente.TipoEmision.Nombre,
        x.IdDomicilio,
        DomicilioMapper.ATexto(x.Domicilio),
        x.Valor,
        x.Fuente.ValorFactorEmision,
        CalculadoraHuella.CalcularCo2Kg(x.Valor, x.Fuente.ValorFactorEmision));
}
