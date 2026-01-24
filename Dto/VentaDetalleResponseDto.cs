public class VentaDetalleResponseDto
{
    public Guid Id { get; set; }

    public Guid InventarioProductoId { get; set; }

    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }

    public List<PagoResponseDto> Pagos { get; set; } = new();
}
