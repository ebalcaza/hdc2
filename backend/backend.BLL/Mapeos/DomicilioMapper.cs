using backend.BLL.DTOs;
using backend.DAL.Entities;

namespace backend.BLL.Mapeos;

internal static class DomicilioMapper
{
    public static Domicilio ANuevaEntidad(DomicilioRequest r, long? idUsuario = null)
    {
        var ahora = DateTime.UtcNow;
        return new Domicilio
        {
            Calle = r.Calle.Trim(),
            Numero = r.Altura.Trim(),
            Departamento = r.Departamento?.Trim(),
            Block = r.Block?.Trim(),
            Manzana = r.Manzana?.Trim(),
            Piso = r.Piso?.Trim(),
            Ciudad = r.Ciudad.Trim(),
            Distrito = r.Distrito.Trim(),
            Provincia = r.Provincia.Trim(),
            CodigoPostal = r.CodigoPostal.Trim(),
            Telefono = r.Telefono?.Trim(),
            FechaAlta = ahora,
            FechaModificacion = ahora,
            AutorModificacion = idUsuario
        };
    }

    public static DomicilioDto ADto(Domicilio d) => new(
        d.IdDomicilio, d.Calle, d.Numero, d.Departamento, d.Block, d.Manzana, d.Piso,
        d.Ciudad, d.Distrito, d.Provincia, d.CodigoPostal, d.Telefono);

    /// <summary>Texto corto para mostrar en reportes: "Calle 123, Ciudad".</summary>
    public static string ATexto(Domicilio d)
    {
        var partes = new[] { $"{d.Calle} {d.Numero}".Trim(), d.Ciudad ?? string.Empty };
        return string.Join(", ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
