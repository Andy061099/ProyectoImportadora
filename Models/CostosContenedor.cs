using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ImportadoraApi.Models
{
    public class CostosContenedor
    {
        public Guid Id { get; set; }

        public Guid Idcotenedor { get; set; }

        public TipoMoneda Tipo { get; set; }

        public decimal Monto { get; set; }

        public Contenedor contenedorasignado { get; set; } = null!;

    }

}