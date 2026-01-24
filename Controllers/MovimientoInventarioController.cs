using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
using ImportadoraApi.Responses;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientoInventarioController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/MovimientoInventario
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movimientos = await _context.MovimientosInventario
                .AsNoTracking()
                .ToListAsync();

            var result = movimientos.Select(m => new MovimientoInventarioResponseDto
            {
                Id = m.Id,
                InventarioProductoId = m.InventarioProductoId,
                TipoMovimiento = m.TipoMovimiento,
                Fecha = m.Fecha,
                Cantidad = m.Cantidad,
                StockAnterior = m.StockAnterior,
                StockPosterior = m.StockPosterior,
                Origen = m.OrSigen,
                ReferenciaId = m.ReferenciaId,
                Observaciones = m.Observaciones,
                UsuarioId = m.UsuarioId
            }).ToList();

            return Ok(ApiResponse<List<MovimientoInventarioResponseDto>>
                .Ok(result, "Movimientos de inventario obtenidos correctamente"));
        }

        // =========================
        // GET: api/MovimientoInventario/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var m = await _context.MovimientosInventario
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (m == null)
                return NotFound(ApiResponse<string>.Fail(
                    $"Movimiento con id {id} no encontrado",
                    "MOVIMIENTO_404"
                ));

            var result = new MovimientoInventarioResponseDto
            {
                Id = m.Id,
                InventarioProductoId = m.InventarioProductoId,
                TipoMovimiento = m.TipoMovimiento,
                Fecha = m.Fecha,
                Cantidad = m.Cantidad,
                StockAnterior = m.StockAnterior,
                StockPosterior = m.StockPosterior,
                Origen = m.OrSigen,
                ReferenciaId = m.ReferenciaId,
                Observaciones = m.Observaciones,
                UsuarioId = m.UsuarioId
            };

            return Ok(ApiResponse<MovimientoInventarioResponseDto>
                .Ok(result, "Movimiento obtenido correctamente"));
        }

        // // =========================
        // // DELETE: api/MovimientoInventario/{id}
        // // =========================
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> Delete(Guid id)
        // {
        //     var m = await _context.MovimientosInventario
        //         .FirstOrDefaultAsync(m => m.Id == id);

        //     if (m == null)
        //         return NotFound(ApiResponse<string>.Fail(
        //             $"Movimiento con id {id} no encontrado",
        //             "MOVIMIENTO_404"
        //         ));

        //     _context.MovimientosInventario.Remove(m);
        //     await _context.SaveChangesAsync();

        //     return Ok(ApiResponse<string>.Ok(
        //         "OK",
        //         "Movimiento eliminado correctamente"
        //     ));
        // }
    }
}
