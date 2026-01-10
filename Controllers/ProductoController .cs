using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;

namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/producto/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProductoById(Guid id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound($"Producto con id {id} no encontrado");

            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProducto([FromBody] Producto producto)
        {

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            // Validación básica
            if (producto == null)
                return BadRequest("El producto es obligatorio.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(producto.Codigo))
                return BadRequest("El código es obligatorio.");

            // Verificar código duplicado
            var existeCodigo = await _context.Productos
                .AnyAsync(p => p.Codigo == producto.Codigo);

            if (existeCodigo)
                return Conflict("Ya existe un producto con ese código.");

            // Generar Id
            producto.Id = Guid.NewGuid();

            // Guardar
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Retornar 201 Created
            return CreatedAtAction(
                nameof(GetProductoById),
                new { id = producto.Id },
                producto
            );
        }
        // =========================
        // PUT: api/productos/{id}
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(Guid id, [FromBody] Producto producto)
        {
            if (producto == null)
                return BadRequest("El producto es obligatorio.");

            if (id != producto.Id)
                return BadRequest("El id de la URL no coincide con el id del cuerpo.");

            var productoDb = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoDb == null)
                return NotFound($"Producto con id {id} no encontrado.");

            var existeCodigo = await _context.Productos
                       .AnyAsync(p => p.Codigo == producto.Codigo && p.Id != id);

            if (existeCodigo)
                return Conflict("Ya existe un producto con ese código.");

            // Actualizar campos
            productoDb.Codigo = producto.Codigo;
            productoDb.Nombre = producto.Nombre;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.UnidadMedida = producto.UnidadMedida;
            productoDb.CostoUnitario = producto.CostoUnitario;
            productoDb.PrecioMayorista = producto.PrecioMayorista;
            productoDb.PrecioMinorista = producto.PrecioMinorista;

            await _context.SaveChangesAsync();

            return Ok(); // 204
        }

    }

}
