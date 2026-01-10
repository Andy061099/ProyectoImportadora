public class DistribucionProductoCreateDto
{

    public Guid OrigenId { get; set; }

    public Guid ProductoId { get; set; }

    public Guid AlmacenId { get; set; }

    public decimal Cantidad { get; set; }
}
