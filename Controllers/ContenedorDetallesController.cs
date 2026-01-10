using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;

namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContenedorDetallesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContenedorDetallesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContenedorDetalle>>> GetDetalles()
        {
            return await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Include(d => d.Contenedor)
                .OrderByDescending(d => d.Contenedor.FechaArribo)
                .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ContenedorDetalle>> GetDetalle(Guid id)
        {
            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Include(d => d.Contenedor)
                .Include(d => d.Distribuciones)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detalle == null)
                return NotFound("Detalle no encontrado.");

            return Ok(detalle);
        }

        [HttpGet("contenedor/{contenedorId}")]
        public async Task<ActionResult<IEnumerable<ContenedorDetalle>>>
            GetDetallesPorContenedor(Guid contenedorId)
        {
            return await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Where(d => d.ContenedorId == contenedorId)
                .ToListAsync();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDetalle([FromBody] ContenedorDetalle dto)
        {
            if (dto.CantidadRecibida <= 0)
                return BadRequest("La cantidad recibida debe ser mayor que cero.");

            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == dto.ContenedorId);

            if (contenedor == null)
                return BadRequest("Contenedor no existe.");

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == dto.ProductoId);

            if (producto == null)
                return BadRequest("Producto no existe.");

            var detalle = new ContenedorDetalle
            {
                Id = Guid.NewGuid(),
                ContenedorId = dto.ContenedorId,
                ProductoId = dto.ProductoId,
                CantidadRecibida = dto.CantidadRecibida,
                Cantidadactual = dto.CantidadRecibida, // CLAVE
                Packa = dto.Packa,
                Cantproductosxpacka = dto.Cantproductosxpacka,
                Cantidadmerma = dto.Cantidadmerma,
                CostoUnitario = dto.CostoUnitario
            };

            _context.ContenedorDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDetalle),
                new { id = detalle.Id },
                detalle
            );
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetalle(Guid id, [FromBody] ContenedorDetalle dto)
        {
            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Distribuciones)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detalle == null)
                return NotFound();

            if (detalle.Distribuciones.Any())
                return BadRequest("No se puede modificar un detalle ya distribuido.");

            detalle.CantidadRecibida = dto.CantidadRecibida;
            detalle.Cantidadactual = dto.CantidadRecibida;
            detalle.CostoUnitario = dto.CostoUnitario;
            detalle.Cantidadmerma = dto.Cantidadmerma;

            await _context.SaveChangesAsync();
            return Ok();
        }





    }
}