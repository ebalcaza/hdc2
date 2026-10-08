using backend.BLL.DTOs;
using backend.BLL.Mapeos;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface IFuenteEmisionService
{
    Task<List<FuenteEmisionDto>> ListarAsync(CancellationToken ct = default);
}

public class FuenteEmisionService : IFuenteEmisionService
{
    private readonly IFuenteEmisionRepository _fuentes;
    public FuenteEmisionService(IFuenteEmisionRepository fuentes) => _fuentes = fuentes;

    public async Task<List<FuenteEmisionDto>> ListarAsync(CancellationToken ct = default)
    {
        var fuentes = await _fuentes.ListarAsync(ct);
        return fuentes.Select(f => new FuenteEmisionDto(
            f.IdFuente, f.Nombre, f.IdUnidad, f.Unidad.Nombre,
            f.IdAlcance, f.Alcance?.Nombre, f.IdTipoEmision, f.TipoEmision.Nombre,
            f.ValorFactorEmision)).ToList();
    }
}

public interface IDomicilioService
{
    Task<List<DomicilioDto>> ListarAsync(long idUsuario, CancellationToken ct = default);
    Task<DomicilioDto> CrearAsync(long idUsuario, DomicilioRequest request, CancellationToken ct = default);
}

public class DomicilioService : IDomicilioService
{
    private readonly IDomicilioRepository _domicilios;
    public DomicilioService(IDomicilioRepository domicilios) => _domicilios = domicilios;

    public async Task<List<DomicilioDto>> ListarAsync(long idUsuario, CancellationToken ct = default)
    {
        var domicilios = await _domicilios.ListarPorUsuarioAsync(idUsuario, ct);
        return domicilios.Select(DomicilioMapper.ADto).ToList();
    }

    public async Task<DomicilioDto> CrearAsync(long idUsuario, DomicilioRequest request, CancellationToken ct = default)
    {
        var domicilio = DomicilioMapper.ANuevaEntidad(request, idUsuario);
        domicilio.IdUsuario = idUsuario;

        await _domicilios.AgregarAsync(domicilio, ct);
        await _domicilios.GuardarCambiosAsync(ct);

        return DomicilioMapper.ADto(domicilio);
    }
}
