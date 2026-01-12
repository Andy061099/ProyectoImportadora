public class DistribucionProductoResponseDto
{
    public Guid Id { get; set; }

    public Guid ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;

    public Guid AlmacenId { get; set; }
    public string NombreAlmacen { get; set; } = null!;

    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
}
