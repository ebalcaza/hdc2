using backend.BLL.DTOs;
using backend.DAL.Repositories;

namespace backend.BLL.Mapeos;

internal static class EstadisticaMapper
{
    /// <summary>Agrega a cada ítem el porcentaje que representa sobre el total.</summary>
    public static List<ItemEstadisticaDto> ConPorcentaje(List<ItemEstadistica> items, decimal total) =>
        items.Select(i => new ItemEstadisticaDto(
            i.Etiqueta,
            i.TotalCo2Kg,
            i.CantidadRegistros,
            total > 0 ? decimal.Round(i.TotalCo2Kg / total * 100m, 2) : 0m)).ToList();
}
