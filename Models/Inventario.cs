

namespace ImportadoraApi.Models
{
    public class Inventario
    {

        public Guid Id { get; set; }

        public Guid AlmacenId { get; set; }
        public virtual Almacen? Almacen { get; set; } = null!;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<InventarioProducto> Productos { get; set; } = new List<InventarioProducto>();

    }
}