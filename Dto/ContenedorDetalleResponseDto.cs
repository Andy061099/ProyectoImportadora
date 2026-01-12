public class ContenedorDetalleResponseDto
{
    public Guid Id { get; set; }

    public Guid ContenedorId { get; set; }
    public string CodigoContenedor { get; set; } = null!;

    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;

    public decimal CantidadRecibida { get; set; }
    public decimal CantidadActual { get; set; }

    public bool Packa { get; set; }
    public decimal CantProductosPorPacka { get; set; }

    public decimal CantidadMerma { get; set; }
    public decimal CostoUnitario { get; set; }
}
