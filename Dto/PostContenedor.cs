using System.ComponentModel.DataAnnotations;


public class PostContenedorDto
{
    [Required]
    public string Codigo { get; set; } = null!;

    [Required]
    public DateTime FechaArribo { get; set; }

    [Required]
    public string NombreContenedor { get; set; } = null!;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CostoCompraContenedor { get; set; }

    [Required]
    public TipoMoneda Moneda { get; set; }

    public string? Descripcion { get; set; }
}

