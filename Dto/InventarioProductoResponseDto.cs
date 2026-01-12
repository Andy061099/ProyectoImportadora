public class InventarioProductoResponseDto
{
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;
    public decimal StockActual { get; set; }
}
