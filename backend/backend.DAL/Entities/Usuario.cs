namespace backend.DAL.Entities;

public class Usuario
{
    public long IdUsuario { get; set; }
    public long IdRol { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;   // hash BCrypt, nunca texto plano
    public string Nombre { get; set; } = string.Empty;
    public string? Apellido { get; set; }
    public string? Telefono { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public long? AutorUltimaModificacion { get; set; }
    public DateTime? FechaUltimaModificacion { get; set; }

    public Rol Rol { get; set; } = null!;
    public Individuo? Individuo { get; set; }
    public Organizacion? Organizacion { get; set; }
    public ICollection<Domicilio> Domicilios { get; set; } = new List<Domicilio>();
    public ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}

public class Organizacion
{
    public long IdUsuario { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string Cuit { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int? CantidadMiembros { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public ICollection<Individuo> Miembros { get; set; } = new List<Individuo>();
}

public class Individuo
{
    public long IdUsuario { get; set; }
    public long? IdOrganizacion { get; set; }
    public string? Dni { get; set; }
    public string? Genero { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Organizacion? Organizacion { get; set; }
}

public class Domicilio
{
    public long IdDomicilio { get; set; }
    public long IdUsuario { get; set; }
    public string? Calle { get; set; }
    public string? Numero { get; set; }          // "altura" en el glosario
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? Distrito { get; set; }
    public string? Departamento { get; set; }
    public string? Piso { get; set; }
    public string? Block { get; set; }
    public string? Manzana { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public DateTime FechaModificacion { get; set; }
    public long? AutorModificacion { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
