
namespace ImportadoraApi.Models
{
    public class CostoContenedorCreateDto
    {
        public Guid ContenedorDetalleId { get; set; }
        public TipoMoneda Tipo { get; set; }
        public decimal Monto { get; set; }
        public string? Observaciones { get; set; }
    }
}