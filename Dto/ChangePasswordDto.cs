namespace ImportadoraApi.DTOs.Auth
{
    public class ChangePasswordDto
    {
        public string PasswordActual { get; set; } = null!;
        public string PasswordNueva { get; set; } = null!;
    }
}
