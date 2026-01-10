
namespace ImportadoraApi.Models
{
    public class CostoContenedorCreateDto
    {
        public Guid ContenedorId { get; set; }
        public TipoMoneda Tipo { get; set; }
        public decimal Monto { get; set; }
        public string? Observaciones { get; set; }
    }
}