namespace ImportadoraApi.Models
{

    public class Consignacion
    {
        public Guid Id { get; set; }

        public Guid VentaId { get; set; }
        public Venta Venta { get; set; } = null!;

        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }

        public EstadoConsigacion Estado { get; set; } = EstadoConsigacion.ABIERTA;

        public ICollection<ConsignacionMovimiento> Movimientos { get; set; } = new List<ConsignacionMovimiento>();
    }
}