namespace ImportadoraApi.Models
{
    public class Venta
    {
        public Guid Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public TipoVenta TipoVenta { get; set; }

        public Guid AlmacenId { get; set; }
        public Almacen Almacen { get; set; } = null!;

        public string? Cliente { get; set; }

        public decimal Total { get; set; }
        public decimal TotalPagado { get; set; }

        public string Estado { get; set; } = "PAGADA";

        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();

        public Consignacion? Consignacion { get; set; }

        public Guid? Consignacionid { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;



    }
}