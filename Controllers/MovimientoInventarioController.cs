using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;


namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MovimientoInventarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MovimientoInventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventario>>> GetAll()
        {
            return await _context.MovimientosInventario.ToListAsync();
        }

        // GET: api/MovimientoInventario/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventario>> GetById(Guid id)
        {
            var movimientoInventario = await _context.MovimientosInventario
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimientoInventario == null)
                return NotFound();

            return movimientoInventario;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimientoInventario(Guid id)
        {
            var movimientoInventario = await _context.MovimientosInventario
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimientoInventario == null)
                return NotFound();

            _context.MovimientosInventario.Remove(movimientoInventario);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}