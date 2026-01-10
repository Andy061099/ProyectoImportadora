namespace ImportadoraApi.Models
{
    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Rol { get; set; } // Admin, Usuario, Contable, etc.
    }
}
