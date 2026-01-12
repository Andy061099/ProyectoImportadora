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
    public class CostosContenedorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CostosContenedorController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/costoscontenedor/contenedor/{contenedorId}
        // =========================
        [HttpGet("contenedor/{contenedorId}")]
        public async Task<IActionResult> GetCostosPorContenedor(Guid contenedorId)
        {
            var costos = await _context.CostosContenedores
                .Include(c => c.Contenedor)
                .Where(c => c.ContenedorId == contenedorId)
                .Select(c => new CostoContenedorResponseDto
                {
                    Id = c.Id,
                    ContenedorId = c.ContenedorId,
                    CodigoContenedor = c.Contenedor.Codigo,
                    Moneda = c.Moneda,
                    Monto = c.Monto,
                    Observaciones = c.Observaciones
                })
                .ToListAsync();

            return Ok(ApiResponse<List<CostoContenedorResponseDto>>.Ok(costos));
        }

        // =========================
        // POST: api/costoscontenedor
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCosto([FromBody] CostoContenedorCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Datos inválidos"));

            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == dto.ContenedorId);

            if (contenedor == null)
                return BadRequest(ApiResponse<string>.Fail("El contenedor no existe"));

            // 1️⃣ Crear costo
            var costo = new CostosContenedor
            {
                Id = Guid.NewGuid(),
                ContenedorId = dto.ContenedorId,
                Moneda = dto.Moneda,
                Monto = dto.Monto,
                Observaciones = dto.Observaciones
            };

            _context.CostosContenedores.Add(costo);

            // 2️⃣ Registro financiero
            var registro = new RegistroFinanciero
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Tipo = TipoRegistroFinanciero.Gasto,
                Monto = dto.Monto,
                Moneda = dto.Moneda,
                Observaciones = dto.Observaciones,
                ReferenciaTipo = "CostoContenedor",
                ReferenciaId = costo.Id
            };

            _context.RegistrosFinancieros.Add(registro);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Costo agregado correctamente"));
        }

        // =========================
        // DELETE: api/costoscontenedor/{id}
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCosto(Guid id)
        {
            var costo = await _context.CostosContenedores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (costo == null)
                return NotFound(ApiResponse<string>.Fail("Costo no encontrado"));

            // eliminar registro financiero
            var registro = await _context.RegistrosFinancieros
                .FirstOrDefaultAsync(r =>
                    r.ReferenciaTipo == "CostoContenedor" &&
                    r.ReferenciaId == costo.Id);

            if (registro != null)
                _context.RegistrosFinancieros.Remove(registro);

            _context.CostosContenedores.Remove(costo);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Costo eliminado correctamente"));
        }
    }
}
