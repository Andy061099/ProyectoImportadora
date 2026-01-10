
namespace ImportadoraApi.Models
{
    public class CierreDiarioRegistro
    {
        public Guid Id { get; set; }

        public Guid CierreDiarioId { get; set; }
        public CierreDiario CierreDiario { get; set; } = null!;

        public Guid RegistroFinancieroId { get; set; }
        public RegistroFinanciero RegistroFinanciero { get; set; } = null!;
    }
}