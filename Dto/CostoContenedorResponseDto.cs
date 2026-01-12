public class CostoContenedorResponseDto
{
    public Guid Id { get; set; }

    public Guid ContenedorId { get; set; }
    public string CodigoContenedor { get; set; } = null!;

    public TipoMoneda Moneda { get; set; }
    public decimal Monto { get; set; }

    public string? Observaciones { get; set; }
}