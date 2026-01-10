
using System.Security.Cryptography.X509Certificates;
using ImportadoraApi.Models;

public class VentaDetalleCreateDto
{
    public Guid InventarioProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Impuestos { get; set; }

    public ICollection<Pagos> pagos { get; set; } = new List<Pagos>();

}