


namespace ImportadoraApi.Models
{
    public class CierreDiario
    {
        public Guid Id { get; set; }

        public DateTime Fecha { get; set; }        // Día cerrado
        public DateTime FechaCierre { get; set; }  // Momento real

        public Guid? AlmacenId { get; set; }
        public Almacen? Almacen { get; set; } = null!;

        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal GananciaNeta { get; set; }

        public int CantidadVentas { get; set; }

        public ICollection<CierreDiarioRegistro> Registros { get; set; }
            = new List<CierreDiarioRegistro>();
    }
}
