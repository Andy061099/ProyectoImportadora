using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
using ImportadoraApi.Responses;
using System.Security.Cryptography;
using System.Text;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/usuario
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.Activo)
                .AsNoTracking()
                .ToListAsync();

            var result = usuarios.Select(u => new UsuarioResponseDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email,
                Rol = u.Rol,
                Activo = u.Activo
            }).ToList();

            return Ok(ApiResponse<List<UsuarioResponseDto>>.Ok(
                result,
                "Usuarios obtenidos correctamente"
            ));
        }

        // =========================
        // GET: api/usuario/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(Guid id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuario no encontrado",
                    "USUARIO_NOT_FOUND"
                ));

            var result = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Activo = usuario.Activo
            };

            return Ok(ApiResponse<UsuarioResponseDto>.Ok(
                result,
                "Usuario obtenido correctamente"
            ));
        }

        // =========================
        // POST: api/usuario
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(ApiResponse<string>.Fail(
                    "El nombre es obligatorio",
                    "INVALID_NAME"
                ));

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(ApiResponse<string>.Fail(
                    "El email es obligatorio",
                    "INVALID_EMAIL"
                ));

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(ApiResponse<string>.Fail(
                    "La contraseña es obligatoria",
                    "INVALID_PASSWORD"
                ));

            var existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (existeEmail)
                return Conflict(ApiResponse<string>.Fail(
                    "Ya existe un usuario con ese email",
                    "DUPLICATE_EMAIL"
                ));

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Rol = dto.Rol ?? "Usuario",
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var result = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Activo = usuario.Activo
            };

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.Id },
                ApiResponse<UsuarioResponseDto>.Ok(result, "Usuario creado correctamente")
            );
        }

        // =========================
        // PUT: api/usuario/{id}
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(Guid id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuario no encontrado",
                    "USUARIO_NOT_FOUND"
                ));

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == dto.Email && u.Id != id);

                if (existeEmail)
                    return Conflict(ApiResponse<string>.Fail(
                        "Ya existe otro usuario con ese email",
                        "DUPLICATE_EMAIL"
                    ));

                usuario.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                usuario.PasswordHash = HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.Rol))
                usuario.Rol = dto.Rol;



            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Usuario actualizado correctamente"));
        }

        // =========================
        // DELETE: api/usuario/{id}
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(Guid id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound(ApiResponse<string>.Fail(
                    "Usuario no encontrado",
                    "USUARIO_NOT_FOUND"
                ));

            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Usuario desactivado correctamente"));
        }

        // 🔐 HASH PASSWORD
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
