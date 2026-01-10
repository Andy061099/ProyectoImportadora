using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;


namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlmacenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlmacenController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Almacen>>> GetAlmacenes()
        {
            return await _context.Almacenes.Include(k => k.inventario).ThenInclude(k => k.Productos).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Almacen>> GetAlmacenById(Guid id)
        {
            var almacen = await _context.Almacenes.Include(i => i.inventario)
                .FirstOrDefaultAsync(a => a.id == id);

            if (almacen == null)
                return NotFound($"Almacen con id {id} no encontrado");

            return Ok(almacen);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAlmacen([FromBody] Almacen almacen)
        {
            if (almacen == null)
                return BadRequest("El almacén es obligatorio.");

            almacen.id = Guid.NewGuid();

            almacen.inventario = new Inventario
            {
                Id = Guid.NewGuid(),
                AlmacenId = almacen.id

            };

            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAlmacenById),
                new { almacen.id },
                almacen
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlmacen(Guid id, [FromBody] Almacen almacen)
        {
            if (almacen == null)
                return BadRequest("El almacen es obligatorio.");

            if (id != almacen.id)
                return BadRequest("El id de la URL no coincide con el id del cuerpo.");

            var almacenDb = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.id == id);

            if (almacenDb == null)
                return NotFound($"Almacen con id {id} no encontrado.");


            almacenDb.nombre = almacen.nombre;
            almacenDb.nombreencargado = almacen.nombreencargado;
            almacenDb.Ubicacion = almacen.Ubicacion;
            almacenDb.Descripcion = almacen.Descripcion;

            await _context.SaveChangesAsync();

            return Ok();
        }


    }




}