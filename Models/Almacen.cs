
namespace ImportadoraApi.Models
{
    public class Almacen
    {
        public Guid id { get; set; }
        public string nombre { get; set; } = null!;
        public string nombreencargado { get; set; } = null!;

        public string? Ubicacion { get; set; }
        public string? Descripcion { get; set; }
        public Inventario inventario { get; set; } = null!;
    }

}