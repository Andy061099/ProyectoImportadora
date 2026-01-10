using System.ComponentModel.DataAnnotations;

namespace ImportadoraApi.Models
{
    public class PostContenedor
    {


        public string Codigo { get; set; } = null!;
        public DateTime FechaArribo { get; set; }

        public string NombreContenedor { get; set; } = null!;

        public decimal CostoCompraContenedor { get; set; }

        public TipoMoneda moneda { get; set; }

        public String? Descripcion { get; set; }




    }
}