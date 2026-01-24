public class ConsignacionMovimientoCreateDto
{
    public Guid ConsignacionId { get; set; }
    public decimal Monto { get; set; }
    public string? Observaciones { get; set; }
    public Guid UsuarioId { get; set; }
}