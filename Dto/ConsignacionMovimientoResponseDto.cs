public class ConsignacionMovimientoResponseDto
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string? Observaciones { get; set; }

    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = null!;
}