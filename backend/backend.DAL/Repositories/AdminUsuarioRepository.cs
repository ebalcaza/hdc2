using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public record FiltroUsuarios(string? Texto, string? TipoUsuario, bool? Activo);

public interface IAdminUsuarioRepository
{
    Task<(List<Usuario> Items, int Total)> BuscarAsync(FiltroUsuarios filtro, int pagina, int tamano, CancellationToken ct = default);

    /// <summary>Usuario con rol, perfil y domicilios activos. Se devuelve con seguimiento para poder modificarlo.</summary>
    Task<Usuario?> ObtenerAsync(long idUsuario, CancellationToken ct = default);

    Task<Dictionary<long, int>> ContarReportesActivosAsync(List<long> idsUsuario, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public class AdminUsuarioRepository : IAdminUsuarioRepository
{
    private readonly AppDbContext _db;
    public AdminUsuarioRepository(AppDbContext db) => _db = db;

    public async Task<(List<Usuario> Items, int Total)> BuscarAsync(
        FiltroUsuarios f, int pagina, int tamano, CancellationToken ct = default)
    {
        IQueryable<Usuario> q = _db.Usuarios.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(f.TipoUsuario))
        {
            var tipo = f.TipoUsuario;
            q = q.Where(u => u.Rol.Nombre == tipo);
        }

        if (f.Activo == true) q = q.Where(u => u.FechaBaja == null);
        else if (f.Activo == false) q = q.Where(u => u.FechaBaja != null);

        if (!string.IsNullOrWhiteSpace(f.Texto))
        {
            var t = f.Texto.Trim();
            q = q.Where(u =>
                u.Email.Contains(t)
                || u.Nombre.Contains(t)
                || (u.Apellido != null && u.Apellido.Contains(t))
                || (u.Individuo != null && u.Individuo.Dni != null && u.Individuo.Dni.Contains(t))
                || (u.Organizacion != null && (u.Organizacion.RazonSocial.Contains(t) || u.Organizacion.Cuit.Contains(t))));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .Include(u => u.Rol)
            .Include(u => u.Individuo)
            .Include(u => u.Organizacion)
            .OrderByDescending(u => u.IdUsuario)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Usuario?> ObtenerAsync(long idUsuario, CancellationToken ct = default) =>
        _db.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Individuo)
            .Include(u => u.Organizacion)
            .Include(u => u.Domicilios.Where(d => d.FechaBaja == null))
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct);

    public async Task<Dictionary<long, int>> ContarReportesActivosAsync(List<long> idsUsuario, CancellationToken ct = default)
    {
        var filas = await _db.Reportes.AsNoTracking()
            .Where(r => r.FechaBaja == null && idsUsuario.Contains(r.IdUsuario))
            .GroupBy(r => r.IdUsuario)
            .Select(g => new { Id = g.Key, Cantidad = g.Count() })
            .ToListAsync(ct);

        return filas.ToDictionary(x => x.Id, x => x.Cantidad);
    }

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
