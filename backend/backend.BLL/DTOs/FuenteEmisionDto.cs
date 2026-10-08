namespace backend.BLL.DTOs;

public record FuenteEmisionDto(
    long IdFuente,
    string Nombre,
    long IdUnidad,
    string Unidad,
    long? IdAlcance,
    string? Alcance,
    long IdTipoEmision,
    string TipoEmision,
    decimal ValorFactorEmision);
