using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;

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

        // 🔹 GET: api/costoscontenedor/contenedor/{contenedorId}
        [HttpGet("contenedor/{contenedorId}")]
        public async Task<ActionResult<IEnumerable<CostosContenedor>>> GetCostosPorContenedor(Guid contenedorId)
        {
            return await _context.CostosContenedores
                .Where(c => c.Idcotenedor == contenedorId)
                .ToListAsync();
        }

        // 🔹 POST: api/costoscontenedor
        [HttpPost]
        public async Task<IActionResult> CreateCosto([FromBody] CostoContenedorCreateDto dto)
        {
            if (dto.Monto <= 0)
                return BadRequest("El monto debe ser mayor que cero.");

            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == dto.ContenedorId);

            if (contenedor == null)
                return BadRequest("El contenedor no existe.");

            // 1️⃣ Crear costo
            var costo = new CostosContenedor
            {
                Id = Guid.NewGuid(),
                Idcotenedor = dto.ContenedorId,
                Tipo = dto.Tipo,
                Monto = dto.Monto
            };

            _context.CostosContenedores.Add(costo);

            // 2️⃣ Registro financiero (GASTO)
            var registro = new RegistroFinanciero
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.UtcNow,
                Tipo = TipoRegistroFinanciero.Gasto,

                Monto = dto.Monto,
                Moneda = dto.Tipo,
                Observaciones = dto.Observaciones,
                ReferenciaTipo = "CostoContenedor",
                ReferenciaId = costo.Id
            };

            _context.RegistrosFinancieros.Add(registro);

            await _context.SaveChangesAsync();

            return Ok(costo);
        }

        // 🔹 DELETE: api/costoscontenedor/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCosto(Guid id)
        {
            var costo = await _context.CostosContenedores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (costo == null)
                return NotFound("Costo no encontrado.");

            // Eliminar también su registro financiero
            var registro = await _context.RegistrosFinancieros
                .FirstOrDefaultAsync(r =>
                    r.ReferenciaTipo == "CostoContenedor" &&
                    r.ReferenciaId == costo.Id);

            if (registro != null)
                _context.RegistrosFinancieros.Remove(registro);

            _context.CostosContenedores.Remove(costo);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
