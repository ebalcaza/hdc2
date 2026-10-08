using backend.DAL.Data;
using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken ct = default);
    Task<Usuario?> ObtenerPorIdAsync(long idUsuario, CancellationToken ct = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default);
    Task<Rol?> ObtenerRolPorNombreAsync(string nombre, CancellationToken ct = default);
    Task AgregarAsync(Usuario usuario, CancellationToken ct = default);
    Task GuardarCambiosAsync(CancellationToken ct = default);
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;

    public Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken ct = default) =>
        _db.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<Usuario?> ObtenerPorIdAsync(long idUsuario, CancellationToken ct = default) =>
        _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == idUsuario, ct);

    public Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default) =>
        _db.Usuarios.AnyAsync(u => u.Email == email, ct);

    public Task<Rol?> ObtenerRolPorNombreAsync(string nombre, CancellationToken ct = default) =>
        _db.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre, ct);

    public async Task AgregarAsync(Usuario usuario, CancellationToken ct = default) =>
        await _db.Usuarios.AddAsync(usuario, ct);

    public Task GuardarCambiosAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
