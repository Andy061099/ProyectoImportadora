public class MovimientoInventarioResponseDto
{
    public Guid Id { get; set; }
    public Guid InventarioProductoId { get; set; }
    public string ProductoNombre { get; set; } = null!;
    public TipoMovimientoInventario TipoMovimiento { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Cantidad { get; set; }
    public decimal StockAnterior { get; set; }
    public decimal StockPosterior { get; set; }
    public Origen Origen { get; set; }
    public Guid? ReferenciaId { get; set; }
    public string? Observaciones { get; set; }
    public Guid UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = null!;
}