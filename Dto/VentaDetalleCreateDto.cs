public class VentaDetalleCreateDto
{
    public Guid InventarioProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Impuestos { get; set; }

    // 👇 SOLO DESGLOSE
    public List<PagoCreateDto> Pagos { get; set; } = new();
}