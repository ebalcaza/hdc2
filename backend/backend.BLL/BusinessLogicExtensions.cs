using backend.BLL.Security;
using backend.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace backend.BLL;

public static class BusinessLogicExtensions
{
    /// <summary>Registra los servicios de negocio. Requiere AddDataAccess y la sección Jwt configurada.</summary>
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReporteService, ReporteService>();
        services.AddScoped<IFuenteEmisionService, FuenteEmisionService>();
        services.AddScoped<IDomicilioService, DomicilioService>();
        services.AddScoped<IReportePdfService, ReportePdfService>();
        services.AddScoped<IEstadisticaService, EstadisticaService>();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<IAdministradorInicialService, AdministradorInicialService>();
        services.AddScoped<IAdminUsuarioService, AdminUsuarioService>();
        services.AddScoped<IAdminFuenteService, AdminFuenteService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
