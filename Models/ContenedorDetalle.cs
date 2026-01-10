

namespace ImportadoraApi.Models
{
    public class ContenedorDetalle
    {
        public Guid Id { get; set; }

        public Guid ContenedorId { get; set; }
        public Contenedor Contenedor { get; set; } = null!;

        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public decimal CantidadRecibida { get; set; }

        public decimal Cantidadactual { get; set; }

        public bool Packa { get; set; } = false;

        public decimal Cantproductosxpacka { get; set; }

        public decimal CantidaddePacka => CantidadRecibida / Cantproductosxpacka;

        public decimal Cantidadmerma { get; set; }

        public decimal CostoUnitario { get; set; }

        public ICollection<DistribucionProducto> Distribuciones { get; set; }
           = new List<DistribucionProducto>();
    }
}