namespace ImportadoraApi.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!; // Para login
        public string Rol { get; set; } = "Usuario"; // Admin, Usuario, Contable, etc.
        public bool Activo { get; set; } = true;

        // Auditoría: movimientos que realizó
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        public ICollection<Merma> Mermas { get; set; } = new List<Merma>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<RegistroFinanciero> RegistrosFinancieros { get; set; } = new List<RegistroFinanciero>();
        public ICollection<ConsignacionMovimiento> Consignaciones { get; set; } = new List<ConsignacionMovimiento>();
    }
}