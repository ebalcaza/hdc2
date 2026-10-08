namespace backend.DAL.Entities;

/// <summary>Fuente de emisión con su factor (kg de CO2 por unidad de consumo).</summary>
public class FuenteEmision
{
    public long IdFuente { get; set; }
    public long IdTipoEmision { get; set; }
    public long IdUnidad { get; set; }
    public long? IdAlcance { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal ValorFactorEmision { get; set; }

    public TipoEmision TipoEmision { get; set; } = null!;
    public UnidadMedida Unidad { get; set; } = null!;
    public Alcance? Alcance { get; set; }
}

/// <summary>Un reporte de huella de carbono. Está compuesto por muchos registros.</summary>
public class Reporte
{
    public long IdReporte { get; set; }
    public long IdUsuario { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public DateTime? FechaBaja { get; set; }          // baja lógica
    public decimal TotalCo2Kg { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public ICollection<Registro> Registros { get; set; } = new List<Registro>();
}

/// <summary>Un consumo puntual: fuente + domicilio + cantidad consumida.</summary>
public class Registro
{
    public long IdRegistro { get; set; }
    public long IdReporte { get; set; }
    public long IdDomicilio { get; set; }
    public long IdFuente { get; set; }
    public decimal Valor { get; set; }                // consumo, en la unidad de la fuente

    public Reporte Reporte { get; set; } = null!;
    public Domicilio Domicilio { get; set; } = null!;
    public FuenteEmision Fuente { get; set; } = null!;
}
