using backend.DAL.Data;
using backend.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace backend.DAL;

public static class DataAccessExtensions
{
    /// <summary>Registra el DbContext y los repositorios.</summary>
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(opciones => opciones.UseSqlServer(connectionString));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAdminUsuarioRepository, AdminUsuarioRepository>();
        services.AddScoped<IDomicilioRepository, DomicilioRepository>();
        services.AddScoped<IFuenteEmisionRepository, FuenteEmisionRepository>();
        services.AddScoped<IReporteRepository, ReporteRepository>();
        services.AddScoped<IEstadisticaRepository, EstadisticaRepository>();
        services.AddScoped<ICatalogoRepository, CatalogoRepository>();

        return services;
    }
}
