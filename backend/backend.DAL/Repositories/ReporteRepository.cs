using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public interface IReporteRepository
{
    /// <summary>Reportes activos del usuario (sin el detalle de cada registro).</summary>
    Task<List<Reporte>> ListarPorUsuarioAsync(long idUsuario, CancellationToken ct = default);

    /// <summary>Reporte activo del usuario con todos sus registros, fuentes y domicilios.</summary>
    Task<Reporte?> ObtenerAsync(long idReporte, long idUsuario, CancellationToken ct = default);

    Task AgregarAsync(Reporte reporte, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public class ReporteRepository : IReporteRepository
{
    private readonly AppDbContext _db;
    public ReporteRepository(AppDbContext db) => _db = db;

    public Task<List<Reporte>> ListarPorUsuarioAsync(long idUsuario, CancellationToken ct = default) =>
        _db.Reportes.AsNoTracking()
            .Include(r => r.Registros)
            .Where(r => r.IdUsuario == idUsuario && r.FechaBaja == null)
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync(ct);

    public Task<Reporte?> ObtenerAsync(long idReporte, long idUsuario, CancellationToken ct = default) =>
        _db.Reportes
            .Include(r => r.Registros).ThenInclude(x => x.Fuente).ThenInclude(f => f.Unidad)
            .Include(r => r.Registros).ThenInclude(x => x.Fuente).ThenInclude(f => f.TipoEmision)
            .Include(r => r.Registros).ThenInclude(x => x.Fuente).ThenInclude(f => f.Alcance)
            .Include(r => r.Registros).ThenInclude(x => x.Domicilio)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.IdReporte == idReporte && r.IdUsuario == idUsuario && r.FechaBaja == null, ct);

    public async Task AgregarAsync(Reporte reporte, CancellationToken ct = default) =>
        await _db.Reportes.AddAsync(reporte, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
