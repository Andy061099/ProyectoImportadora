
namespace ImportadoraApi.Models
{
    public class MovimientoInventario
    {
        public Guid Id { get; set; }

        public Guid InventarioProductoId { get; set; }
        public InventarioProducto InventarioProducto { get; set; } = null!;

        public TipoMovimientoInventario TipoMovimiento { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public decimal Cantidad { get; set; }

        public decimal StockAnterior { get; set; }
        public decimal StockPosterior { get; set; }

        public Origen OrSigen { get; set; }
        public Guid? ReferenciaId { get; set; }

        public string? Observaciones { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
