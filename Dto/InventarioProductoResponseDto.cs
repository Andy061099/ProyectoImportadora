public class InventarioProductoResponseDto
{
    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;
    public decimal StockActual { get; set; }
    public bool Packa { get; set; }
    public decimal CantidadPorPacka { get; set; }
    public decimal TotalPackas { get; set; }
}
