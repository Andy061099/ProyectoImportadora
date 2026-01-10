
namespace ImportadoraApi.Models
{
    public class Merma
    {
        public Guid Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public Guid AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public decimal Cantidad { get; set; }

        public string Motivo { get; set; } = null!;
    }
}