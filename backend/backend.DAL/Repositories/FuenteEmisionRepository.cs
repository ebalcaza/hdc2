using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public interface IFuenteEmisionRepository
{
    Task<List<FuenteEmision>> ListarAsync(CancellationToken ct = default);
    Task<List<FuenteEmision>> ObtenerPorIdsAsync(List<long> ids, CancellationToken ct = default);

    // ---- Administración de factores de emisión ----
    Task<FuenteEmision?> ObtenerParaEditarAsync(long idFuente, CancellationToken ct = default);
    Task<FuenteEmision?> ObtenerConRelacionesAsync(long idFuente, CancellationToken ct = default);
    Task<HashSet<long>> ObtenerIdsEnUsoAsync(CancellationToken ct = default);
    Task<bool> EstaEnUsoAsync(long idFuente, CancellationToken ct = default);
    Task<bool> ExisteNombreAsync(string nombre, long? excluirId, CancellationToken ct = default);
    Task AgregarAsync(FuenteEmision fuente, CancellationToken ct = default);
    void Eliminar(FuenteEmision fuente);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public class FuenteEmisionRepository : IFuenteEmisionRepository
{
    private readonly AppDbContext _db;
    public FuenteEmisionRepository(AppDbContext db) => _db = db;

    public Task<List<FuenteEmision>> ListarAsync(CancellationToken ct = default) =>
        _db.FuentesEmision.AsNoTracking()
            .Include(f => f.Unidad)
            .Include(f => f.TipoEmision)
            .Include(f => f.Alcance)
            .OrderBy(f => f.Nombre)
            .ToListAsync(ct);

    public Task<List<FuenteEmision>> ObtenerPorIdsAsync(List<long> ids, CancellationToken ct = default) =>
        _db.FuentesEmision.AsNoTracking()
            .Where(f => ids.Contains(f.IdFuente))
            .ToListAsync(ct);

    public Task<FuenteEmision?> ObtenerParaEditarAsync(long idFuente, CancellationToken ct = default) =>
        _db.FuentesEmision.FirstOrDefaultAsync(f => f.IdFuente == idFuente, ct);

    public Task<FuenteEmision?> ObtenerConRelacionesAsync(long idFuente, CancellationToken ct = default) =>
        _db.FuentesEmision.AsNoTracking()
            .Include(f => f.Unidad)
            .Include(f => f.TipoEmision)
            .Include(f => f.Alcance)
            .FirstOrDefaultAsync(f => f.IdFuente == idFuente, ct);

    /// <summary>Fuentes que ya tienen registros (incluidos los de reportes dados de baja).</summary>
    public async Task<HashSet<long>> ObtenerIdsEnUsoAsync(CancellationToken ct = default)
    {
        var ids = await _db.Registros.AsNoTracking().Select(r => r.IdFuente).Distinct().ToListAsync(ct);
        return ids.ToHashSet();
    }

    public Task<bool> EstaEnUsoAsync(long idFuente, CancellationToken ct = default) =>
        _db.Registros.AnyAsync(r => r.IdFuente == idFuente, ct);

    public Task<bool> ExisteNombreAsync(string nombre, long? excluirId, CancellationToken ct = default) =>
        _db.FuentesEmision.AnyAsync(f => f.Nombre == nombre && (excluirId == null || f.IdFuente != excluirId), ct);

    public async Task AgregarAsync(FuenteEmision fuente, CancellationToken ct = default) =>
        await _db.FuentesEmision.AddAsync(fuente, ct);

    public void Eliminar(FuenteEmision fuente) => _db.FuentesEmision.Remove(fuente);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
