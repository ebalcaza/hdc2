using System.ComponentModel.DataAnnotations;

namespace backend.BLL.DTOs;

public class DomicilioRequest
{
    [Required, MaxLength(100)] public string Calle { get; set; } = string.Empty;
    [Required, MaxLength(20)] public string Altura { get; set; } = string.Empty;
    [MaxLength(20)] public string? Departamento { get; set; }
    [MaxLength(20)] public string? Block { get; set; }
    [MaxLength(20)] public string? Manzana { get; set; }
    [MaxLength(20)] public string? Piso { get; set; }
    [Required, MaxLength(200)] public string Ciudad { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string Distrito { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Provincia { get; set; } = string.Empty;
    [Required, MaxLength(10)] public string CodigoPostal { get; set; } = string.Empty;
    [MaxLength(20)] public string? Telefono { get; set; }
}

public record DomicilioDto(
    long IdDomicilio,
    string? Calle,
    string? Altura,
    string? Departamento,
    string? Block,
    string? Manzana,
    string? Piso,
    string? Ciudad,
    string? Distrito,
    string? Provincia,
    string? CodigoPostal,
    string? Telefono);
