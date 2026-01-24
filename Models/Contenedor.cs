

namespace ImportadoraApi.Models
{
    public class Contenedor
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = null!;
        public DateTime FechaArribo { get; set; }
        public string NombreContenedor { get; set; } = null!;
        public EstadoContenedor Estado { get; set; } = EstadoContenedor.EnProceso;
        public ICollection<CostosContenedor> Costos { get; set; } = new List<CostosContenedor>();
        public ICollection<ContenedorDetalle> Detalles { get; set; } = new List<ContenedorDetalle>();
    }
}