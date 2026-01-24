public class ProductoCreateDto
{
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string UnidadMedida { get; set; } = null!;
    public decimal CostoUnitario { get; set; }
    public decimal PrecioMayorista { get; set; }
    public decimal PrecioMinorista { get; set; }
    public TipoMoneda MonedaDeEntrada { get; set; }
}
