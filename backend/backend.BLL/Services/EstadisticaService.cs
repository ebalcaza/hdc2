using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.BLL.Mapeos;
using backend.BLL.Security;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IEstadisticaService
{
    Task<EstadisticaDto> GenerarAsync(FiltroEstadisticaRequest request, CancellationToken ct = default);
}

public class EstadisticaService : IEstadisticaService
{
    private readonly IEstadisticaRepository _estadisticas;
    public EstadisticaService(IEstadisticaRepository estadisticas) => _estadisticas = estadisticas;

    // RFR02: reporte estadístico del administrador según los filtros elegidos
    public async Task<EstadisticaDto> GenerarAsync(FiltroEstadisticaRequest request, CancellationToken ct = default)
    {
        if (request.Desde is not null && request.Hasta is not null && request.Desde > request.Hasta)
            throw new BusinessRuleException("La fecha 'Desde' no puede ser posterior a 'Hasta'.");

        if (!string.IsNullOrWhiteSpace(request.TipoUsuario)
            && request.TipoUsuario != Roles.Individuo
            && request.TipoUsuario != Roles.Organizacion)
        {
            throw new BusinessRuleException("El tipo de usuario debe ser 'Individuo' u 'Organizacion'.");
        }

        var filtro = new FiltroEstadistica(
            request.Desde,
            request.Hasta,
            string.IsNullOrWhiteSpace(request.TipoUsuario) ? null : request.TipoUsuario,
            request.IdFuente,
            request.IdAlcance,
            request.IdTipoEmision,
            string.IsNullOrWhiteSpace(request.Provincia) ? null : request.Provincia.Trim(),
            string.IsNullOrWhiteSpace(request.Ciudad) ? null : request.Ciudad.Trim());

        // Se ejecutan de a una: un DbContext no admite consultas en paralelo
        var resumen = await _estadisticas.ObtenerResumenAsync(filtro, ct);
        var porFuente = await _estadisticas.PorFuenteAsync(filtro, ct);
        var porAlcance = await _estadisticas.PorAlcanceAsync(filtro, ct);
        var porTipoEmision = await _estadisticas.PorTipoEmisionAsync(filtro, ct);
        var porProvincia = await _estadisticas.PorProvinciaAsync(filtro, ct);
        var porTipoUsuario = await _estadisticas.PorTipoUsuarioAsync(filtro, ct);
        var porMes = await _estadisticas.PorMesAsync(filtro, ct);

        var total = resumen.TotalCo2Kg;

        return new EstadisticaDto(
            DateTime.UtcNow,
            request,
            resumen.CantidadReportes,
            resumen.CantidadRegistros,
            total,
            EstadisticaMapper.ConPorcentaje(porFuente, total),
            EstadisticaMapper.ConPorcentaje(porAlcance, total),
            EstadisticaMapper.ConPorcentaje(porTipoEmision, total),
            EstadisticaMapper.ConPorcentaje(porProvincia, total),
            EstadisticaMapper.ConPorcentaje(porTipoUsuario, total),
            EstadisticaMapper.ConPorcentaje(porMes, total));
    }
}
