
using ImportadoraApi.Models;
using ImportadoraApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContenedorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContenedorController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/contenedor
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetContenedores()
        {
            var contenedores = await _context.Contenedores
                .OrderByDescending(c => c.FechaArribo)
                .Select(c => new ContenedorResponseDto
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    NombreContenedor = c.NombreContenedor,
                    FechaArribo = c.FechaArribo,
                    Estado = c.Estado

                })
                .ToListAsync();

            return Ok(ApiResponse<List<ContenedorResponseDto>>.Ok(contenedores));
        }

        // =========================
        // GET: api/contenedor/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContenedor(Guid id)
        {
            var contenedor = await _context.Contenedores
                .Where(c => c.Id == id)
                .Select(c => new ContenedorResponseDto
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    NombreContenedor = c.NombreContenedor,
                    FechaArribo = c.FechaArribo,
                    Estado = c.Estado
                })
                .FirstOrDefaultAsync();

            if (contenedor == null)
                return NotFound(ApiResponse<string>.Fail("Contenedor no encontrado"));

            return Ok(ApiResponse<ContenedorResponseDto>.Ok(contenedor));
        }

        // =========================
        // POST: api/contenedor
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateContenedor([FromBody] PostContenedorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Datos inválidos"));

            var existeCodigo = await _context.Contenedores
                .AnyAsync(c => c.Codigo == dto.Codigo);

            if (existeCodigo)
                return Conflict(ApiResponse<string>.Fail("Ya existe un contenedor con ese código"));

            var idContenedor = Guid.NewGuid();

            var contenedor = new Contenedor
            {
                Id = idContenedor,
                Codigo = dto.Codigo,
                NombreContenedor = dto.NombreContenedor,
                FechaArribo = dto.FechaArribo,
                Estado = EstadoContenedor.EnProceso
            };

            _context.Contenedores.Add(contenedor);

            _context.RegistrosFinancieros.Add(new RegistroFinanciero
            {
                Id = Guid.NewGuid(),
                Moneda = dto.Moneda,
                Monto = dto.CostoCompraContenedor,
                Observaciones = dto.Descripcion,
                ReferenciaTipo = "Compra Contenedor",
                ReferenciaId = idContenedor,
                Tipo = TipoRegistroFinanciero.Gasto
            });

            await _context.SaveChangesAsync();

            var response = new ContenedorResponseDto
            {
                Id = contenedor.Id,
                Codigo = contenedor.Codigo,
                NombreContenedor = contenedor.NombreContenedor,
                FechaArribo = contenedor.FechaArribo,
                Estado = contenedor.Estado
            };

            return Ok(ApiResponse<ContenedorResponseDto>.Ok(response, "Contenedor creado correctamente"));
        }

        // =========================
        // PUT: api/contenedor/{id}
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContenedor(Guid id, [FromBody] PostContenedorDto dto)
        {
            var contenedorDb = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contenedorDb == null)
                return NotFound(ApiResponse<string>.Fail("Contenedor no encontrado"));

            if (contenedorDb.Codigo != dto.Codigo)
            {
                var existeCodigo = await _context.Contenedores
                    .AnyAsync(c => c.Id != id && c.Codigo == dto.Codigo);

                if (existeCodigo)
                    return Conflict(ApiResponse<string>.Fail("Ya existe un contenedor con ese código"));
            }

            contenedorDb.Codigo = dto.Codigo;
            contenedorDb.NombreContenedor = dto.NombreContenedor;
            contenedorDb.FechaArribo = dto.FechaArribo;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Contenedor actualizado correctamente"));
        }
    }
}
