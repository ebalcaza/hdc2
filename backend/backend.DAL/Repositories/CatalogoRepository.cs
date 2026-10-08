using backend.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public record ItemCatalogo(long Id, string Nombre);

public interface ICatalogoRepository
{
    Task<List<ItemCatalogo>> ListarAlcancesAsync(CancellationToken ct = default);
    Task<List<ItemCatalogo>> ListarTiposEmisionAsync(CancellationToken ct = default);
    Task<List<ItemCatalogo>> ListarUnidadesAsync(CancellationToken ct = default);
}

public class CatalogoRepository : ICatalogoRepository
{
    private readonly AppDbContext _db;
    public CatalogoRepository(AppDbContext db) => _db = db;

    public Task<List<ItemCatalogo>> ListarAlcancesAsync(CancellationToken ct = default) =>
        _db.Alcances.AsNoTracking().OrderBy(x => x.IdAlcance)
            .Select(x => new ItemCatalogo(x.IdAlcance, x.Nombre)).ToListAsync(ct);

    public Task<List<ItemCatalogo>> ListarTiposEmisionAsync(CancellationToken ct = default) =>
        _db.TiposEmision.AsNoTracking().OrderBy(x => x.IdTipoEmision)
            .Select(x => new ItemCatalogo(x.IdTipoEmision, x.Nombre)).ToListAsync(ct);

    public Task<List<ItemCatalogo>> ListarUnidadesAsync(CancellationToken ct = default) =>
        _db.UnidadesMedida.AsNoTracking().OrderBy(x => x.IdUnidad)
            .Select(x => new ItemCatalogo(x.IdUnidad, x.Nombre)).ToListAsync(ct);
}
