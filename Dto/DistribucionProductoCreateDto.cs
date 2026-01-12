using System.ComponentModel.DataAnnotations;

public class DistribucionProductoCreateDto
{
    [Required]
    public Guid OrigenId { get; set; } // ContenedorDetalleId

    [Required]
    public Guid ProductoId { get; set; }

    [Required]
    public Guid AlmacenId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Cantidad { get; set; }
}
