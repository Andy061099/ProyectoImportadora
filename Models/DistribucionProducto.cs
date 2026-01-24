namespace ImportadoraApi.Models
{
    public class DistribucionProducto
    {
        public Guid Id { get; set; }

        public Guid OrigenId { get; set; }

        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public Guid AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public decimal Cantidad { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public Guid UsuarioId { get; set; }

    }
}