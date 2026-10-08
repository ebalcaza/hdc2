using System.Globalization;
using backend.BLL.DTOs;
using backend.BLL.Mapeos;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IDashboardService
{
    Task<DashboardDto> ObtenerAsync(long idUsuario, CancellationToken ct = default);
}

/// <summary>Panel principal del usuario: totales, desglose por alcance y evolución en el tiempo.</summary>
public class DashboardService : IDashboardService
{
    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    private readonly IEstadisticaRepository _estadisticas;
    private readonly IReporteRepository _reportes;

    public DashboardService(IEstadisticaRepository estadisticas, IReporteRepository reportes)
    {
        _estadisticas = estadisticas;
        _reportes = reportes;
    }

    public async Task<DashboardDto> ObtenerAsync(long idUsuario, CancellationToken ct = default)
    {
        // Mismo cálculo que las estadísticas del administrador, pero limitado a este usuario
        var filtro = new FiltroEstadistica(null, null, null, null, null, null, null, null, idUsuario);

        // Se ejecutan de a una: un DbContext no admite consultas en paralelo
        var resumen = await _estadisticas.ObtenerResumenAsync(filtro, ct);
        var porAlcance = await _estadisticas.PorAlcanceAsync(filtro, ct);
        var porTipoEmision = await _estadisticas.PorTipoEmisionAsync(filtro, ct);
        var porFuente = await _estadisticas.PorFuenteAsync(filtro, ct);
        var porMes = await _estadisticas.PorMesAsync(filtro, ct);
        var ultimo = (await _reportes.ListarPorUsuarioAsync(idUsuario, ct)).FirstOrDefault();   // vienen del más nuevo al más viejo

        var total = resumen.TotalCo2Kg;
        var serie = ArmarSerie(porMes);

        var hoy = DateTime.UtcNow;
        var esteMes = TotalDelMes(serie, hoy);
        var mesAnterior = TotalDelMes(serie, hoy.AddMonths(-1));
        decimal? variacion = mesAnterior > 0 ? decimal.Round((esteMes - mesAnterior) / mesAnterior * 100m, 1) : null;

        var promedio = resumen.CantidadReportes > 0 ? decimal.Round(total / resumen.CantidadReportes, 4) : 0m;

        return new DashboardDto(
            DateTime.UtcNow,
            total,
            resumen.CantidadReportes,
            resumen.CantidadRegistros,
            promedio,
            ultimo?.FechaCreacion,
            ultimo?.TotalCo2Kg,
            esteMes,
            mesAnterior,
            variacion,
            EstadisticaMapper.ConPorcentaje(porAlcance, total),
            EstadisticaMapper.ConPorcentaje(porTipoEmision, total),
            EstadisticaMapper.ConPorcentaje(porFuente.Take(5).ToList(), total),
            serie);
    }

    /// <summary>Completa con ceros los meses sin reportes y calcula el acumulado, hasta el mes actual.</summary>
    private static List<PuntoMensualDto> ArmarSerie(List<ItemEstadistica> meses)
    {
        var serie = new List<PuntoMensualDto>();
        if (meses.Count == 0) return serie;

        var totales = meses.ToDictionary(m => m.Etiqueta, m => m.TotalCo2Kg);

        var inicio = ParsearMes(meses.First().Etiqueta);
        var hoy = DateTime.UtcNow;
        var fin = new DateTime(hoy.Year, hoy.Month, 1);
        var ultimo = ParsearMes(meses.Last().Etiqueta);
        if (ultimo > fin) fin = ultimo;

        decimal acumulado = 0;
        for (var mes = inicio; mes <= fin; mes = mes.AddMonths(1))
        {
            var clave = mes.ToString("yyyy-MM", Invariante);
            totales.TryGetValue(clave, out var totalMes);
            acumulado += totalMes;
            serie.Add(new PuntoMensualDto(clave, totalMes, acumulado));
        }

        return serie;
    }

    private static DateTime ParsearMes(string etiqueta) =>
        DateTime.ParseExact($"{etiqueta}-01", "yyyy-MM-dd", Invariante);

    private static decimal TotalDelMes(List<PuntoMensualDto> serie, DateTime fecha)
    {
        var clave = fecha.ToString("yyyy-MM", Invariante);
        return serie.FirstOrDefault(p => p.Mes == clave)?.TotalCo2Kg ?? 0m;
    }
}
