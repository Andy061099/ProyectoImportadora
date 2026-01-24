
public class MermaCreateDto
{
    public Guid AlmacenId { get; set; }

    public Guid ProductoId { get; set; }


    public decimal Cantidad { get; set; }

    public string Motivo { get; set; } = null!;

    public Guid UsuarioId { get; set; }
}