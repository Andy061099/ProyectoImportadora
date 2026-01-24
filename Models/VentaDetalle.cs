namespace ImportadoraApi.Models
{
    public class VentaDetalle
    {
        public Guid Id { get; set; }

        public Guid VentaId { get; set; }
        public Venta Venta { get; set; } = null!;

        public Guid InventarioProductoId { get; set; }
        public InventarioProducto InventarioProducto { get; set; } = null!;

        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public decimal Impuestos { get; set; }

        public ICollection<Pagos> Pagos { get; set; } = new List<Pagos>();

    }
}