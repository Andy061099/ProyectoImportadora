public class AlmacenResponseDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string? NombreEncargado { get; set; }
    public string? Ubicacion { get; set; }
    public string? Descripcion { get; set; }

    public Guid InventarioId { get; set; }
}
