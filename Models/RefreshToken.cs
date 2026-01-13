namespace ImportadoraApi.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Token { get; set; } = null!;
        public DateTime Expira { get; set; }
        public bool Revocado { get; set; } = false;
    }
}
