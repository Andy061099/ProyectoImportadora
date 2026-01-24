public class VentaCreateDto
{
    public TipoVenta TipoVenta { get; set; }
    public Guid AlmacenId { get; set; }
    public string? Cliente { get; set; }
    public Guid UsuarioId { get; set; }

    public decimal TotalPagado { get; set; }

    public TipoMoneda MonedaDeclarada { get; set; }

    public List<VentaDetalleCreateDto> Detalles { get; set; } = new();
}