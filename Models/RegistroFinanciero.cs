
namespace ImportadoraApi.Models
{
    public class RegistroFinanciero
    {
        public Guid Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public TipoRegistroFinanciero Tipo { get; set; }



        public Guid? AlmacenId { get; set; }
        public Almacen? Almacen { get; set; } = null!;

        public decimal Monto { get; set; }

        public string ReferenciaTipo { get; set; } = null!;

        public TipoMoneda Moneda { get; set; }

        public Guid? ReferenciaId { get; set; }

        public string? Observaciones { get; set; }

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}