using ImportadoraApi.DTOs.Auth;
using ImportadoraApi.Models;
using ImportadoraApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =========================
        // LOGIN
        // =========================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Activo);

            if (user == null)
                return Unauthorized("Credenciales incorrectas");

            if (!PasswordHelper.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciales incorrectas");

            var token = GenerateToken(user);

            return Ok(new
            {
                success = true,
                message = "Login exitoso",
                data = new
                {
                    token,
                    user.Id,
                    user.Nombre,
                    user.Email,
                    user.Rol
                }
            });
        }

        // =========================
        // CAMBIO DE CONTRASEÑA
        // =========================
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return Unauthorized();

            if (!PasswordHelper.Verify(dto.PasswordActual, user.PasswordHash))
                return BadRequest("La contraseña actual es incorrecta");

            user.PasswordHash = PasswordHelper.Hash(dto.PasswordNueva);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contraseña actualizada correctamente"
            });
        }

        // =========================
        // RECUPERAR CONTRASEÑA (BÁSICO)
        // =========================
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordDto dto)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Activo);

            if (user == null)
                return Ok(new { success = true, message = "Si el correo existe, se enviará recuperación" });

            // 🔴 Aquí luego puedes integrar email real
            user.PasswordHash = PasswordHelper.Hash("123456");
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Contraseña reiniciada. Revisa tu correo (simulado: 123456)"
            });
        }

        // =========================
        // TOKEN
        // =========================
        private string GenerateToken(Usuario user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
