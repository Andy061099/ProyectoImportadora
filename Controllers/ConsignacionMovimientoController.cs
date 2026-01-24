using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using ImportadoraApi.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsignacionMovimientoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsignacionMovimientoController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // POST: api/consignacionmovimiento
        // =========================
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> CreateMovimiento(
            [FromBody] ConsignacionMovimientoCreateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<object>.Fail(
                    "El cuerpo es obligatorio",
                    "INVALID_BODY"
                ));

            if (dto.Monto <= 0)
                return BadRequest(ApiResponse<object>.Fail(
                    "El monto debe ser mayor que cero",
                    "INVALID_AMOUNT"
                ));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var consignacion = await _context.Consignaciones
                    .Include(c => c.Movimientos)
                    .FirstOrDefaultAsync(c => c.Id == dto.ConsignacionId);

                if (consignacion == null)
                    return NotFound(ApiResponse<object>.Fail(
                        "Consignación no encontrada",
                        "CONSIGNACION_NOT_FOUND"
                    ));

                if (consignacion.Estado == EstadoConsigacion.CERRADA)
                    return BadRequest(ApiResponse<object>.Fail(
                        "La consignación ya está cerrada",
                        "CONSIGNACION_CLOSED"
                    ));

                if (dto.Monto > consignacion.MontoPendiente)
                    return BadRequest(ApiResponse<object>.Fail(
                        "El monto excede el pendiente",
                        "AMOUNT_EXCEEDS_PENDING"
                    ));

                // =========================
                // CREAR MOVIMIENTO
                // =========================
                var movimiento = new ConsignacionMovimiento
                {
                    Id = Guid.NewGuid(),
                    ConsignacionId = consignacion.Id,
                    Monto = dto.Monto,
                    Observaciones = dto.Observaciones,
                    UsuarioId = dto.UsuarioId,
                    Fecha = DateTime.UtcNow
                };

                _context.ConsignacionMovimientos.Add(movimiento);

                // =========================
                // ACTUALIZAR CONSIGNACIÓN
                // =========================
                consignacion.MontoPendiente -= dto.Monto;

                if (consignacion.MontoPendiente <= 0)
                {
                    consignacion.Estado = EstadoConsigacion.CERRADA;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Ok(
                    new { movimiento.Id },
                    "Movimiento de consignación registrado correctamente"
                ));
            }
            catch
            {
                await transaction.RollbackAsync();

                return StatusCode(500, ApiResponse<object>.Fail(
                    "Error interno al registrar el movimiento",
                    "INTERNAL_ERROR"
                ));
            }
        }
        [HttpGet("consignacion/{consignacionId}")]
        public async Task<ActionResult<ApiResponse<List<ConsignacionMovimientoResponseDto>>>>
        GetByConsignacion(Guid consignacionId)
        {
            var movimientos = await _context.ConsignacionMovimientos
                .AsNoTracking()
                .Include(m => m.Usuario)
                .Where(m => m.ConsignacionId == consignacionId)
                .OrderBy(m => m.Fecha)
                .Select(m => new ConsignacionMovimientoResponseDto
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    Monto = m.Monto,
                    Observaciones = m.Observaciones,
                    UsuarioId = m.UsuarioId,
                    UsuarioNombre = m.Usuario.Nombre
                })
                .ToListAsync();

            return Ok(ApiResponse<List<ConsignacionMovimientoResponseDto>>.Ok(
                movimientos,
                "Movimientos de consignación obtenidos correctamente"
            ));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ConsignacionMovimientoResponseDto>>>
    GetById(Guid id)
        {
            var movimiento = await _context.ConsignacionMovimientos
                .AsNoTracking()
                .Include(m => m.Usuario)
                .Where(m => m.Id == id)
                .Select(m => new ConsignacionMovimientoResponseDto
                {
                    Id = m.Id,
                    Fecha = m.Fecha,
                    Monto = m.Monto,
                    Observaciones = m.Observaciones,
                    UsuarioId = m.UsuarioId,
                    UsuarioNombre = m.Usuario.Nombre
                })
                .FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound(ApiResponse<ConsignacionMovimientoResponseDto>.Fail(
                    $"Movimiento con id {id} no encontrado",
                    "CONSIGNACION_MOVIMIENTO_NOT_FOUND"
                ));

            return Ok(ApiResponse<ConsignacionMovimientoResponseDto>.Ok(
                movimiento,
                "Movimiento obtenido correctamente"
            ));
        }

    }
}
