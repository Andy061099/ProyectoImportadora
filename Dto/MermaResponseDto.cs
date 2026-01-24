public class MermaResponseDto
{
    public Guid MermaId { get; set; }
    public string Producto { get; set; } = null!;
    public string Almacen { get; set; } = null!;
    public decimal Cantidad { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = null!;
    public decimal StockAnterior { get; set; }
    public decimal StockPosterior { get; set; }
}