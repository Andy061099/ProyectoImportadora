
namespace ImportadoraApi.Models
{
    public class ConsignacionMovimiento
    {
        public Guid Id { get; set; }

        public Guid ConsignacionId { get; set; }
        public Consignacion Consignacion { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public decimal Monto { get; set; }

        public string? Observaciones { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}