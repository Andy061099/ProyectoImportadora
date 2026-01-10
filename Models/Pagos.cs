using System.ComponentModel.DataAnnotations;

namespace ImportadoraApi.Models
{
    public class Pagos
    {
        public Guid id { get; set; }
        public decimal cantidad { get; set; }

        public TipoMoneda tipoMoneda { get; set; }


    }
}