using System.ComponentModel.DataAnnotations;

public class PostContenedorDetalleDto
{
    [Required]
    public Guid ContenedorId { get; set; }

    [Required]
    public Guid ProductoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal CantidadRecibida { get; set; }

    public bool Packa { get; set; } = false;

    [Range(0, double.MaxValue)]
    public decimal CantProductosPorPacka { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CantidadMerma { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CostoUnitario { get; set; }
}
