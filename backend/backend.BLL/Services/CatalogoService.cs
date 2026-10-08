using backend.BLL.DTOs;
using backend.DAL.Repositories;

namespace backend.BLL.Services;

public interface ICatalogoService
{
    Task<List<ItemCatalogoDto>> ListarAlcancesAsync(CancellationToken ct = default);
    Task<List<ItemCatalogoDto>> ListarTiposEmisionAsync(CancellationToken ct = default);
    Task<List<ItemCatalogoDto>> ListarUnidadesAsync(CancellationToken ct = default);
}

public class CatalogoService : ICatalogoService
{
    private readonly ICatalogoRepository _catalogos;
    public CatalogoService(ICatalogoRepository catalogos) => _catalogos = catalogos;

    public async Task<List<ItemCatalogoDto>> ListarAlcancesAsync(CancellationToken ct = default) =>
        Mapear(await _catalogos.ListarAlcancesAsync(ct));

    public async Task<List<ItemCatalogoDto>> ListarTiposEmisionAsync(CancellationToken ct = default) =>
        Mapear(await _catalogos.ListarTiposEmisionAsync(ct));

    public async Task<List<ItemCatalogoDto>> ListarUnidadesAsync(CancellationToken ct = default) =>
        Mapear(await _catalogos.ListarUnidadesAsync(ct));

    private static List<ItemCatalogoDto> Mapear(List<ItemCatalogo> items) =>
        items.Select(i => new ItemCatalogoDto(i.Id, i.Nombre)).ToList();
}
