using System.ComponentModel.DataAnnotations;

namespace ImportadoraApi.Models
{
    public class Pagos
    {
        public Guid Id { get; set; }

        public Guid VentaDetalleId { get; set; }
        public VentaDetalle VentaDetalle { get; set; } = null!;

        public decimal Cantidad { get; set; }
        public TipoMoneda TipoMoneda { get; set; }
    }
}