public class ContenedorResponseDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string NombreContenedor { get; set; } = null!;
    public DateTime FechaArribo { get; set; }
    public EstadoContenedor Estado { get; set; }
}
