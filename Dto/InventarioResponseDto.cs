public class InventarioResponseDto
{
    public Guid Id { get; set; }

    public Guid AlmacenId { get; set; }
    public string NombreAlmacen { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public List<InventarioProductoResponseDto> Productos { get; set; } = new();
}
