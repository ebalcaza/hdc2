using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.BLL.Mapeos;
using backend.DAL.Entities;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IReporteService
{
    Task<List<ReporteResumenDto>> ListarAsync(long idUsuario, CancellationToken ct = default);
    Task<ReporteDetalleDto> ObtenerAsync(long idUsuario, long idReporte, CancellationToken ct = default);
    Task<ReporteDetalleDto> CrearAsync(long idUsuario, ReporteRequest request, CancellationToken ct = default);
    Task<ReporteDetalleDto> ActualizarAsync(long idUsuario, long idReporte, ReporteRequest request, CancellationToken ct = default);
    Task EliminarAsync(long idUsuario, long idReporte, CancellationToken ct = default);
}

public class ReporteService : IReporteService
{
    private readonly IReporteRepository _reportes;
    private readonly IFuenteEmisionRepository _fuentes;
    private readonly IDomicilioRepository _domicilios;

    public ReporteService(
        IReporteRepository reportes,
        IFuenteEmisionRepository fuentes,
        IDomicilioRepository domicilios)
    {
        _reportes = reportes;
        _fuentes = fuentes;
        _domicilios = domicilios;
    }

    // RFR05: lista de reportes del usuario
    public async Task<List<ReporteResumenDto>> ListarAsync(long idUsuario, CancellationToken ct = default)
    {
        var reportes = await _reportes.ListarPorUsuarioAsync(idUsuario, ct);
        return reportes.Select(ReporteMapper.AResumen).ToList();
    }

    // RFR06: detalle de un reporte
    public async Task<ReporteDetalleDto> ObtenerAsync(long idUsuario, long idReporte, CancellationToken ct = default)
    {
        var reporte = await _reportes.ObtenerAsync(idReporte, idUsuario, ct)
            ?? throw new NotFoundException("El reporte no existe.");
        return ReporteMapper.ADetalle(reporte);
    }

    // RFR01: crear reporte con sus registros de consumo
    public async Task<ReporteDetalleDto> CrearAsync(long idUsuario, ReporteRequest request, CancellationToken ct = default)
    {
        var (registros, total) = await ConstruirRegistrosAsync(idUsuario, request, ct);

        var reporte = new Reporte
        {
            IdUsuario = idUsuario,
            FechaCreacion = DateTime.UtcNow,
            TotalCo2Kg = total,
            Registros = registros
        };

        await _reportes.AgregarAsync(reporte, ct);
        await _reportes.GuardarCambiosAsync(ct);

        return await ObtenerAsync(idUsuario, reporte.IdReporte, ct);
    }

    // RFR03: editar los datos de consumo (se reemplaza el conjunto de registros)
    public async Task<ReporteDetalleDto> ActualizarAsync(
        long idUsuario, long idReporte, ReporteRequest request, CancellationToken ct = default)
    {
        var reporte = await _reportes.ObtenerAsync(idReporte, idUsuario, ct)
            ?? throw new NotFoundException("El reporte no existe.");

        var (registros, total) = await ConstruirRegistrosAsync(idUsuario, request, ct);

        // Los registros viejos quedan huérfanos y EF los elimina al guardar
        reporte.Registros.Clear();
        foreach (var registro in registros)
            reporte.Registros.Add(registro);

        reporte.TotalCo2Kg = total;
        reporte.FechaModificacion = DateTime.UtcNow;

        await _reportes.GuardarCambiosAsync(ct);

        return await ObtenerAsync(idUsuario, idReporte, ct);
    }

    // RFR04: eliminar (baja lógica, el reporte deja de listarse)
    public async Task EliminarAsync(long idUsuario, long idReporte, CancellationToken ct = default)
    {
        var reporte = await _reportes.ObtenerAsync(idReporte, idUsuario, ct)
            ?? throw new NotFoundException("El reporte no existe.");

        reporte.FechaBaja = DateTime.UtcNow;
        await _reportes.GuardarCambiosAsync(ct);
    }

    /// <summary>Valida los datos de consumo y calcula el CO2 de cada registro y el total.</summary>
    private async Task<(List<Registro> Registros, decimal Total)> ConstruirRegistrosAsync(
        long idUsuario, ReporteRequest request, CancellationToken ct)
    {
        var idsFuente = request.Registros.Select(r => r.IdFuente).Distinct().ToList();
        var fuentes = (await _fuentes.ObtenerPorIdsAsync(idsFuente, ct)).ToDictionary(f => f.IdFuente);

        if (fuentes.Count != idsFuente.Count)
            throw new BusinessRuleException("Alguna de las fuentes de emisión indicadas no existe.");

        var domiciliosPropios = (await _domicilios.ObtenerIdsPorUsuarioAsync(idUsuario, ct)).ToHashSet();

        var registros = new List<Registro>();
        decimal total = 0;

        foreach (var r in request.Registros)
        {
            if (!domiciliosPropios.Contains(r.IdDomicilio))
                throw new BusinessRuleException($"El domicilio {r.IdDomicilio} no pertenece al usuario.");

            total += CalculadoraHuella.CalcularCo2Kg(r.Valor, fuentes[r.IdFuente].ValorFactorEmision);

            registros.Add(new Registro
            {
                IdFuente = r.IdFuente,
                IdDomicilio = r.IdDomicilio,
                Valor = r.Valor
            });
        }

        return (registros, total);
    }
}
