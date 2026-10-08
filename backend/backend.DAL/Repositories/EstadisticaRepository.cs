using System.Linq.Expressions;
using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

/// <summary>Filtros opcionales para las estadísticas. Todo lo que viene nulo no filtra.</summary>
public record FiltroEstadistica(
    DateTime? Desde,
    DateTime? Hasta,
    string? TipoUsuario,       // nombre del rol: Individuo u Organizacion
    long? IdFuente,
    long? IdAlcance,
    long? IdTipoEmision,
    string? Provincia,
    string? Ciudad,
    long? IdUsuario = null);   // si viene, solo cuenta los reportes de ese usuario

public record ResumenEstadistica(int CantidadReportes, int CantidadRegistros, decimal TotalCo2Kg);

public record ItemEstadistica(string Etiqueta, decimal TotalCo2Kg, int CantidadRegistros);

public interface IEstadisticaRepository
{
    Task<ResumenEstadistica> ObtenerResumenAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorFuenteAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorAlcanceAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorTipoEmisionAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorProvinciaAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorTipoUsuarioAsync(FiltroEstadistica filtro, CancellationToken ct = default);
    Task<List<ItemEstadistica>> PorMesAsync(FiltroEstadistica filtro, CancellationToken ct = default);
}

public class EstadisticaRepository : IEstadisticaRepository
{
    private readonly AppDbContext _db;
    public EstadisticaRepository(AppDbContext db) => _db = db;

    public async Task<ResumenEstadistica> ObtenerResumenAsync(FiltroEstadistica filtro, CancellationToken ct = default)
    {
        var q = Base(filtro);
        var registros = await q.CountAsync(ct);
        var reportes = await q.Select(x => x.IdReporte).Distinct().CountAsync(ct);
        var total = await q.SumAsync(x => (decimal?)(x.Valor * x.Fuente.ValorFactorEmision), ct) ?? 0m;
        return new ResumenEstadistica(reportes, registros, decimal.Round(total, 4));
    }

    public async Task<List<ItemEstadistica>> PorFuenteAsync(FiltroEstadistica filtro, CancellationToken ct = default) =>
        ComoItems(await AgruparAsync(Base(filtro), x => x.Fuente.Nombre, ct));

    public async Task<List<ItemEstadistica>> PorAlcanceAsync(FiltroEstadistica filtro, CancellationToken ct = default) =>
        ComoItems(await AgruparAsync(Base(filtro),
            x => x.Fuente.Alcance == null ? "Sin alcance" : x.Fuente.Alcance.Nombre, ct));

    public async Task<List<ItemEstadistica>> PorTipoEmisionAsync(FiltroEstadistica filtro, CancellationToken ct = default) =>
        ComoItems(await AgruparAsync(Base(filtro), x => x.Fuente.TipoEmision.Nombre, ct));

    public async Task<List<ItemEstadistica>> PorProvinciaAsync(FiltroEstadistica filtro, CancellationToken ct = default) =>
        ComoItems(await AgruparAsync(Base(filtro), x => x.Domicilio.Provincia ?? "Sin dato", ct));

    public async Task<List<ItemEstadistica>> PorTipoUsuarioAsync(FiltroEstadistica filtro, CancellationToken ct = default) =>
        ComoItems(await AgruparAsync(Base(filtro), x => x.Reporte.Usuario.Rol.Nombre, ct));

    public async Task<List<ItemEstadistica>> PorMesAsync(FiltroEstadistica filtro, CancellationToken ct = default)
    {
        var filas = await AgruparAsync(
            Base(filtro),
            x => x.Reporte.FechaCreacion.Year * 100 + x.Reporte.FechaCreacion.Month,
            ct);

        // Orden cronológico, etiqueta "2026-10"
        return filas
            .OrderBy(f => f.Clave)
            .Select(f => new ItemEstadistica($"{f.Clave / 100}-{f.Clave % 100:00}", f.Total, f.Cantidad))
            .ToList();
    }

    /// <summary>Registros de reportes activos que cumplen los filtros.</summary>
    private IQueryable<Registro> Base(FiltroEstadistica f)
    {
        var q = _db.Registros.AsNoTracking().Where(x => x.Reporte.FechaBaja == null);

        if (f.Desde is not null)
        {
            var desde = f.Desde.Value.Date;
            q = q.Where(x => x.Reporte.FechaCreacion >= desde);
        }

        if (f.Hasta is not null)
        {
            var hastaExclusivo = f.Hasta.Value.Date.AddDays(1);   // incluye todo el día "Hasta"
            q = q.Where(x => x.Reporte.FechaCreacion < hastaExclusivo);
        }

        if (!string.IsNullOrWhiteSpace(f.TipoUsuario))
        {
            var tipo = f.TipoUsuario;
            q = q.Where(x => x.Reporte.Usuario.Rol.Nombre == tipo);
        }

        if (f.IdUsuario is not null)
        {
            var idUsuario = f.IdUsuario;
            q = q.Where(x => x.Reporte.IdUsuario == idUsuario);
        }

        if (f.IdFuente is not null) q = q.Where(x => x.IdFuente == f.IdFuente);
        if (f.IdAlcance is not null) q = q.Where(x => x.Fuente.IdAlcance == f.IdAlcance);
        if (f.IdTipoEmision is not null) q = q.Where(x => x.Fuente.IdTipoEmision == f.IdTipoEmision);

        if (!string.IsNullOrWhiteSpace(f.Provincia))
        {
            var provincia = f.Provincia;
            q = q.Where(x => x.Domicilio.Provincia == provincia);
        }

        if (!string.IsNullOrWhiteSpace(f.Ciudad))
        {
            var ciudad = f.Ciudad;
            q = q.Where(x => x.Domicilio.Ciudad == ciudad);
        }

        return q;
    }

    /// <summary>Agrupa en la base de datos y suma consumo × factor de emisión.</summary>
    private static async Task<List<(TKey Clave, decimal Total, int Cantidad)>> AgruparAsync<TKey>(
        IQueryable<Registro> q, Expression<Func<Registro, TKey>> clave, CancellationToken ct)
    {
        var filas = await q
            .GroupBy(clave)
            .Select(g => new
            {
                g.Key,
                Total = g.Sum(x => x.Valor * x.Fuente.ValorFactorEmision),
                Cantidad = g.Count()
            })
            .ToListAsync(ct);

        return filas.Select(f => (f.Key, decimal.Round(f.Total, 4), f.Cantidad)).ToList();
    }

    private static List<ItemEstadistica> ComoItems(List<(string Clave, decimal Total, int Cantidad)> filas) =>
        filas.OrderByDescending(f => f.Total)
            .Select(f => new ItemEstadistica(f.Clave, f.Total, f.Cantidad))
            .ToList();
}
