using backend.BLL.DTOs;
using backend.BLL.Exceptions;
using backend.DAL.Entities;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IAdminFuenteService
{
    Task<List<FuenteEmisionAdminDto>> ListarAsync(CancellationToken ct = default);
    Task<FuenteEmisionAdminDto> CrearAsync(FuenteEmisionRequest request, CancellationToken ct = default);
    Task<FuenteEmisionAdminDto> ActualizarAsync(long idFuente, FuenteEmisionRequest request, CancellationToken ct = default);
    Task EliminarAsync(long idFuente, CancellationToken ct = default);
}

/// <summary>El administrador ingresa y mantiene las fuentes y sus factores de emisión.</summary>
public class AdminFuenteService : IAdminFuenteService
{
    private readonly IFuenteEmisionRepository _fuentes;
    private readonly ICatalogoRepository _catalogos;

    public AdminFuenteService(IFuenteEmisionRepository fuentes, ICatalogoRepository catalogos)
    {
        _fuentes = fuentes;
        _catalogos = catalogos;
    }

    public async Task<List<FuenteEmisionAdminDto>> ListarAsync(CancellationToken ct = default)
    {
        var fuentes = await _fuentes.ListarAsync(ct);
        var enUso = await _fuentes.ObtenerIdsEnUsoAsync(ct);
        return fuentes.Select(f => ADto(f, enUso.Contains(f.IdFuente))).ToList();
    }

    // Agregar un nuevo factor de emisión
    public async Task<FuenteEmisionAdminDto> CrearAsync(FuenteEmisionRequest request, CancellationToken ct = default)
    {
        await ValidarAsync(request, null, ct);

        var fuente = new FuenteEmision
        {
            Nombre = request.Nombre.Trim(),
            IdUnidad = request.IdUnidad,
            IdTipoEmision = request.IdTipoEmision,
            IdAlcance = request.IdAlcance,
            ValorFactorEmision = request.ValorFactorEmision
        };

        await _fuentes.AgregarAsync(fuente, ct);
        await _fuentes.GuardarCambiosAsync(ct);

        return await ObtenerDtoAsync(fuente.IdFuente, ct);
    }

    public async Task<FuenteEmisionAdminDto> ActualizarAsync(long idFuente, FuenteEmisionRequest request, CancellationToken ct = default)
    {
        var fuente = await _fuentes.ObtenerParaEditarAsync(idFuente, ct)
            ?? throw new NotFoundException("La fuente de emisión no existe.");

        await ValidarAsync(request, idFuente, ct);

        // El CO2 de un reporte se calcula con el factor vigente: cambiarlo alteraría reportes ya creados
        var cambiaCalculo = fuente.ValorFactorEmision != request.ValorFactorEmision || fuente.IdUnidad != request.IdUnidad;
        if (cambiaCalculo && await _fuentes.EstaEnUsoAsync(idFuente, ct))
        {
            throw new BusinessRuleException(
                "Esta fuente ya se usa en reportes: no se puede cambiar su factor ni su unidad. " +
                "Creá una nueva fuente con el valor actualizado.");
        }

        fuente.Nombre = request.Nombre.Trim();
        fuente.IdUnidad = request.IdUnidad;
        fuente.IdTipoEmision = request.IdTipoEmision;
        fuente.IdAlcance = request.IdAlcance;
        fuente.ValorFactorEmision = request.ValorFactorEmision;
        await _fuentes.GuardarCambiosAsync(ct);

        return await ObtenerDtoAsync(idFuente, ct);
    }

    public async Task EliminarAsync(long idFuente, CancellationToken ct = default)
    {
        var fuente = await _fuentes.ObtenerParaEditarAsync(idFuente, ct)
            ?? throw new NotFoundException("La fuente de emisión no existe.");

        if (await _fuentes.EstaEnUsoAsync(idFuente, ct))
            throw new BusinessRuleException("No se puede eliminar: la fuente ya tiene registros en reportes.");

        _fuentes.Eliminar(fuente);
        await _fuentes.GuardarCambiosAsync(ct);
    }

    private async Task ValidarAsync(FuenteEmisionRequest r, long? excluirId, CancellationToken ct)
    {
        if (await _fuentes.ExisteNombreAsync(r.Nombre.Trim(), excluirId, ct))
            throw new BusinessRuleException("Ya existe una fuente de emisión con ese nombre.");

        if (!(await _catalogos.ListarUnidadesAsync(ct)).Any(x => x.Id == r.IdUnidad))
            throw new BusinessRuleException("La unidad de medida no existe.");

        if (!(await _catalogos.ListarTiposEmisionAsync(ct)).Any(x => x.Id == r.IdTipoEmision))
            throw new BusinessRuleException("El tipo de emisión no existe.");

        if (r.IdAlcance is not null && !(await _catalogos.ListarAlcancesAsync(ct)).Any(x => x.Id == r.IdAlcance))
            throw new BusinessRuleException("El alcance no existe.");
    }

    private async Task<FuenteEmisionAdminDto> ObtenerDtoAsync(long idFuente, CancellationToken ct)
    {
        var fuente = await _fuentes.ObtenerConRelacionesAsync(idFuente, ct)
            ?? throw new NotFoundException("La fuente de emisión no existe.");
        return ADto(fuente, await _fuentes.EstaEnUsoAsync(idFuente, ct));
    }

    private static FuenteEmisionAdminDto ADto(FuenteEmision f, bool enUso) => new(
        f.IdFuente, f.Nombre, f.IdUnidad, f.Unidad.Nombre, f.IdAlcance, f.Alcance?.Nombre,
        f.IdTipoEmision, f.TipoEmision.Nombre, f.ValorFactorEmision, enUso);
}
