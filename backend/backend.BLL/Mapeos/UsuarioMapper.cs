using backend.BLL.DTOs;
using backend.DAL.Entities;

namespace backend.BLL.Mapeos;

internal static class UsuarioMapper
{
    /// <summary>Organización: razón social. Resto: "Nombre Apellido".</summary>
    public static string NombreVisible(Usuario u) =>
        u.Organizacion is not null ? u.Organizacion.RazonSocial : $"{u.Nombre} {u.Apellido}".Trim();

    public static UsuarioResumenDto AResumen(Usuario u, int cantidadReportes) => new(
        u.IdUsuario, u.Email, NombreVisible(u), u.Rol.Nombre,
        u.FechaBaja is null, u.FechaAlta, u.FechaBaja, cantidadReportes);

    public static UsuarioDetalleDto ADetalle(Usuario u, int cantidadReportes) => new(
        u.IdUsuario, u.Email, NombreVisible(u), u.Rol.Nombre,
        u.FechaBaja is null, u.FechaAlta, u.FechaBaja, cantidadReportes,
        u.Apellido, u.Telefono, u.FechaNacimiento,
        u.Individuo?.Dni, u.Individuo?.Genero,
        u.Organizacion?.RazonSocial, u.Organizacion?.Cuit, u.Organizacion?.Area,
        u.Organizacion?.Descripcion, u.Organizacion?.CantidadMiembros,
        u.Domicilios.OrderBy(d => d.IdDomicilio).Select(DomicilioMapper.ADto).ToList());
}
