namespace ImportadoraApi.Models
{
    public class CostosContenedor
    {
        public Guid Id { get; set; }

        public Guid ContenedorId { get; set; }
        public Contenedor Contenedor { get; set; } = null!;

        public TipoMoneda Moneda { get; set; }
        public decimal Monto { get; set; }

        public string? Observaciones { get; set; }
    }
}
