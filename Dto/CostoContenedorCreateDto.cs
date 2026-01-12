
using System.ComponentModel.DataAnnotations;

public class CostoContenedorCreateDto
{
    [Required]
    public Guid ContenedorId { get; set; }

    [Required]
    public TipoMoneda Moneda { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Monto { get; set; }

    public string? Observaciones { get; set; }
}
