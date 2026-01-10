namespace ImportadoraApi.Models
{
    public class InventarioProducto
    {

        public Guid Id { get; set; }

        public Guid InventarioId { get; set; }
        public Inventario Inventario { get; set; } = null!;

        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public decimal StockActual { get; set; }


        public bool Packa { get; set; } = false;

        public decimal Cantproductosxpacka { get; set; }

        public decimal CantidaddePacka => StockActual / Cantproductosxpacka;

        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();

    }
}