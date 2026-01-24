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
                .AsNoTracking()
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

            return Ok(ApiResponse<List<CostoContenedorResponseDto>>.Ok(costos, "Costos obtenidos correctamente"));
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

            if (dto.Monto <= 0)
                return BadRequest(ApiResponse<string>.Fail("El monto debe ser mayor que cero"));

            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == dto.ContenedorId);

            if (contenedor == null)
                return BadRequest(ApiResponse<string>.Fail("El contenedor no existe"));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
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

                // 2️⃣ Crear registro financiero
                var registro = new RegistroFinanciero
                {
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.UtcNow,
                    Tipo = TipoRegistroFinanciero.Gasto,
                    Monto = dto.Monto,
                    MonedaDeclarada = dto.Moneda,
                    Observaciones = dto.Observaciones,
                    ReferenciaTipo = "CostoContenedor",
                    ReferenciaId = costo.Id
                };

                _context.RegistrosFinancieros.Add(registro);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new CostoContenedorResponseDto
                {
                    Id = costo.Id,
                    ContenedorId = costo.ContenedorId,
                    CodigoContenedor = contenedor.Codigo,
                    Moneda = costo.Moneda,
                    Monto = costo.Monto,
                    Observaciones = costo.Observaciones
                };

                return Ok(ApiResponse<CostoContenedorResponseDto>.Ok(response, "Costo agregado correctamente"));
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<string>.Fail("Error interno al registrar el costo", "INTERNAL_ERROR"));
            }
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

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // eliminar registro financiero asociado
                var registro = await _context.RegistrosFinancieros
                    .FirstOrDefaultAsync(r =>
                        r.ReferenciaTipo == "CostoContenedor" &&
                        r.ReferenciaId == costo.Id);

                if (registro != null)
                    _context.RegistrosFinancieros.Remove(registro);

                _context.CostosContenedores.Remove(costo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new CostoContenedorResponseDto
                {
                    Id = costo.Id,
                    ContenedorId = costo.ContenedorId,
                    CodigoContenedor = (await _context.Contenedores.FindAsync(costo.ContenedorId))?.Codigo ?? "",
                    Moneda = costo.Moneda,
                    Monto = costo.Monto,
                    Observaciones = costo.Observaciones
                };

                return Ok(ApiResponse<CostoContenedorResponseDto>.Ok(response, "Costo eliminado correctamente"));
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<string>.Fail("Error interno al eliminar el costo", "INTERNAL_ERROR"));
            }
        }
    }
}
