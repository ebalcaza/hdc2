using backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.DAL.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<TipoEmision> TiposEmision => Set<TipoEmision>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Alcance> Alcances => Set<Alcance>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Organizacion> Organizaciones => Set<Organizacion>();
    public DbSet<Individuo> Individuos => Set<Individuo>();
    public DbSet<Domicilio> Domicilios => Set<Domicilio>();
    public DbSet<FuenteEmision> FuentesEmision => Set<FuenteEmision>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<Registro> Registros => Set<Registro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
