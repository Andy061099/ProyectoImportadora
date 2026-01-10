
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;




namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistribucionProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DistribucionProductoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistribucionProducto>>> GetDistribuciones()
        {
            return await _context.DistribucionProductos
                .Include(d => d.Producto)
                .Include(d => d.Almacen)
                .OrderByDescending(d => d.Fecha)
                .ToListAsync();
        }

        // 🔹 GET: api/distribucionproducto/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DistribucionProducto>> GetDistribucion(Guid id)
        {
            var distribucion = await _context.DistribucionProductos
                .Include(d => d.Producto)
                .Include(d => d.Almacen)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (distribucion == null)
                return NotFound("Distribución no encontrada.");

            return Ok(distribucion);
        }


        [HttpPost]
        public async Task<IActionResult> CrearDistribucion([FromBody] DistribucionProductoCreateDto dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor que cero.");

            decimal cantidadDisponible = 0;

            var detalle = await _context.ContenedorDetalles
                 .Include(d => d.Distribuciones)
                 .FirstOrDefaultAsync(d => d.Id == dto.OrigenId);

            if (detalle == null)
                return BadRequest("Detalle de contenedor no encontrado.");

            if (detalle.ProductoId != dto.ProductoId)
                return BadRequest("El producto no coincide.");

            var yaDistribuido = detalle.Distribuciones.Sum(d => d.Cantidad);
            cantidadDisponible = detalle.CantidadRecibida - yaDistribuido;



            if (dto.Cantidad > cantidadDisponible)
                return BadRequest("La cantidad supera lo disponible.");

            // =========================
            // 2️⃣ CREAR DISTRIBUCIÓN
            // =========================

            var distribucion = new DistribucionProducto
            {
                Id = Guid.NewGuid(),

                OrigenId = dto.OrigenId,
                ProductoId = dto.ProductoId,
                AlmacenId = dto.AlmacenId,
                Cantidad = dto.Cantidad,
                Fecha = DateTime.UtcNow,

            };

            _context.DistribucionProductos.Add(distribucion);

            // =========================
            // 3️⃣ INVENTARIO PRODUCTO
            // =========================

            var inventarioProducto = await _context.InventarioProductos
                .Include(ip => ip.Inventario)
                .FirstOrDefaultAsync(ip =>
                    ip.Inventario.AlmacenId == dto.AlmacenId &&
                    ip.ProductoId == dto.ProductoId);

            if (inventarioProducto == null)
            {
                var inventario = await _context.Inventarios
                    .FirstOrDefaultAsync(i => i.AlmacenId == dto.AlmacenId);

                if (inventario == null)
                    return BadRequest("El almacén no tiene inventario.");

                inventarioProducto = new InventarioProducto
                {
                    Id = Guid.NewGuid(),
                    InventarioId = inventario.Id,
                    ProductoId = dto.ProductoId,
                    StockActual = 0,

                };

                _context.InventarioProductos.Add(inventarioProducto);
            }

            // =========================
            // 4️⃣ MOVIMIENTO INVENTARIO
            // =========================

            var stockAnterior = inventarioProducto.StockActual;
            var stockPosterior = stockAnterior + dto.Cantidad;

            var movimiento = new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                InventarioProductoId = inventarioProducto.Id,
                TipoMovimiento = TipoMovimientoInventario.Entrada,
                Fecha = DateTime.UtcNow,
                Cantidad = dto.Cantidad,
                StockAnterior = stockAnterior,
                StockPosterior = stockPosterior,
                ReferenciaId = dto.OrigenId,
                // UsuarioId = dto.OrigenId,
                Observaciones = "Distribución de producto a almacén"
            };

            inventarioProducto.StockActual = stockPosterior;

            _context.MovimientosInventario.Add(movimiento);

            // =========================
            // 5️⃣ GUARDAR TODO
            // =========================

            await _context.SaveChangesAsync();

            return Ok(distribucion);
        }

    }
}