namespace backend.BLL.Security;

public class JwtSettings
{
    public const string Seccion = "Jwt";

    public string Clave { get; set; } = string.Empty;       // mínimo 32 caracteres
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiraMinutos { get; set; } = 120;
}
