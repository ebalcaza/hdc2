using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public interface IDomicilioRepository
{
    Task<List<Domicilio>> ListarPorUsuarioAsync(long idUsuario, CancellationToken ct = default);
    Task<List<long>> ObtenerIdsPorUsuarioAsync(long idUsuario, CancellationToken ct = default);
    Task AgregarAsync(Domicilio domicilio, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public class DomicilioRepository : IDomicilioRepository
{
    private readonly AppDbContext _db;
    public DomicilioRepository(AppDbContext db) => _db = db;

    public Task<List<Domicilio>> ListarPorUsuarioAsync(long idUsuario, CancellationToken ct = default) =>
        _db.Domicilios.AsNoTracking()
            .Where(d => d.IdUsuario == idUsuario && d.FechaBaja == null)
            .OrderBy(d => d.IdDomicilio)
            .ToListAsync(ct);

    public Task<List<long>> ObtenerIdsPorUsuarioAsync(long idUsuario, CancellationToken ct = default) =>
        _db.Domicilios
            .Where(d => d.IdUsuario == idUsuario && d.FechaBaja == null)
            .Select(d => d.IdDomicilio)
            .ToListAsync(ct);

    public async Task AgregarAsync(Domicilio domicilio, CancellationToken ct = default) =>
        await _db.Domicilios.AddAsync(domicilio, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
