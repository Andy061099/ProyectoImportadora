public class VentaResponseDto
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public TipoVenta TipoVenta { get; set; }
    public Guid AlmacenId { get; set; }
    public string? Cliente { get; set; }

    public Guid Usuario { get; set; }

    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public EstadoVenta Estado { get; set; }

    public TipoMoneda MonedaDeclarada { get; set; }

    public List<VentaDetalleResponseDto> Detalles { get; set; } = new();
}
