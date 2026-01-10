using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace ImportadoraApi.Controllers
{
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
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Where(u => u.Activo)
                .ToListAsync();
        }

        // =========================
        // GET: api/usuario/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(Guid id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            return Ok(usuario);
        }

        // =========================
        // POST: api/usuario
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("El email es obligatorio");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("La contraseña es obligatoria");

            var existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email == dto.Email);

            if (existeEmail)
                return BadRequest("Ya existe un usuario con ese email");

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

            return Ok(usuario);
        }

        // =========================
        // PUT: api/usuario/{id}
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(Guid id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var existeEmail = await _context.Usuarios
                    .AnyAsync(u => u.Email == dto.Email && u.Id != id);

                if (existeEmail)
                    return BadRequest("Ya existe otro usuario con ese email");

                usuario.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                usuario.PasswordHash = HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.Rol))
                usuario.Rol = dto.Rol;

            await _context.SaveChangesAsync();

            return Ok(usuario);
        }


        // =========================
        // 🔐 HASH PASSWORD
        // =========================
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }





    }
}