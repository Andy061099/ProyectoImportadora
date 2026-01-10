using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;

namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventarioProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/InventarioProducto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventarioProducto>>> GetAll()
        {
            return await _context.InventarioProductos
                .Include(ip => ip.Producto)
                .Include(ip => ip.Inventario)
                .ToListAsync();
        }


        // GET: api/InventarioProducto/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InventarioProducto>> GetById(Guid id)
        {
            var inventarioProducto = await _context.InventarioProductos
                .Include(ip => ip.Producto)
                .Include(ip => ip.Inventario)
                .FirstOrDefaultAsync(ip => ip.Id == id);

            if (inventarioProducto == null)
                return NotFound();

            return inventarioProducto;
        }

        // PUT: api/InventarioProducto/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] InventarioProducto inventarioProducto)
        {
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != inventarioProducto.Id)
                return BadRequest("El id de la URL no coincide con el id del cuerpo.");

            var inventarioProductoDb = await _context.InventarioProductos
                .FirstOrDefaultAsync(ip => ip.Id == id);

            if (inventarioProductoDb == null)
                return NotFound();



            inventarioProductoDb.Packa = inventarioProducto.Packa;
            inventarioProductoDb.Cantproductosxpacka = inventarioProducto.Cantproductosxpacka;


            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}