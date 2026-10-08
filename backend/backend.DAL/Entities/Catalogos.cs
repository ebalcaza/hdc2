namespace backend.DAL.Entities;

public class Rol
{
    public long IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Permiso> Permisos { get; set; } = new List<Permiso>();
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}

public class Permiso
{
    public long IdPermiso { get; set; }
    public long IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public Rol Rol { get; set; } = null!;
}

public class TipoEmision
{
    public long IdTipoEmision { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class UnidadMedida
{
    public long IdUnidad { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class Alcance
{
    public long IdAlcance { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
