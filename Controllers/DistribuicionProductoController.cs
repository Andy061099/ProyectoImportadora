using ImportadoraApi.Models;
using ImportadoraApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DistribucionProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DistribucionProductoController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/distribucionproducto
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetDistribuciones()
        {
            var distribuciones = await _context.DistribucionProductos
                .Include(d => d.Producto)
                .Include(d => d.Almacen)
                .OrderByDescending(d => d.Fecha)
                .Select(d => new DistribucionProductoResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    AlmacenId = d.AlmacenId,
                    NombreAlmacen = d.Almacen.nombre,
                    Cantidad = d.Cantidad,
                    Fecha = d.Fecha
                })
                .ToListAsync();

            return Ok(ApiResponse<List<DistribucionProductoResponseDto>>.Ok(distribuciones));
        }

        // =========================
        // GET: api/distribucionproducto/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistribucion(Guid id)
        {
            var distribucion = await _context.DistribucionProductos
                .Include(d => d.Producto)
                .Include(d => d.Almacen)
                .Where(d => d.Id == id)
                .Select(d => new DistribucionProductoResponseDto
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    AlmacenId = d.AlmacenId,
                    NombreAlmacen = d.Almacen.nombre,
                    Cantidad = d.Cantidad,
                    Fecha = d.Fecha
                })
                .FirstOrDefaultAsync();

            if (distribucion == null)
                return NotFound(ApiResponse<string>.Fail("Distribución no encontrada"));

            return Ok(ApiResponse<DistribucionProductoResponseDto>.Ok(distribucion));
        }

        // =========================
        // POST: api/distribucionproducto
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearDistribucion([FromBody] DistribucionProductoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Datos inválidos"));

            // =========================
            // 1️⃣ Validar detalle origen
            // =========================
            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Distribuciones)
                .FirstOrDefaultAsync(d => d.Id == dto.OrigenId);

            if (detalle == null)
                return BadRequest(ApiResponse<string>.Fail("Detalle de contenedor no encontrado"));

            if (detalle.ProductoId != dto.ProductoId)
                return BadRequest(ApiResponse<string>.Fail("El producto no coincide con el detalle de origen"));

            var yaDistribuido = detalle.Distribuciones.Sum(d => d.Cantidad);
            var disponible = detalle.CantidadRecibida - yaDistribuido;

            if (dto.Cantidad > disponible)
                return BadRequest(ApiResponse<string>.Fail("La cantidad supera lo disponible en el contenedor"));

            // =========================
            // 2️⃣ Crear distribución
            // =========================
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var distribucion = new DistribucionProducto
            {
                Id = Guid.NewGuid(),
                OrigenId = dto.OrigenId,
                ProductoId = dto.ProductoId,
                AlmacenId = dto.AlmacenId,
                Cantidad = dto.Cantidad,
                Fecha = DateTime.UtcNow,
                UsuarioId = Guid.Parse(userId!)
            };

            _context.DistribucionProductos.Add(distribucion);

            // =========================
            // 3️⃣ InventarioProducto
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
                    return BadRequest(ApiResponse<string>.Fail("El almacén no tiene inventario creado"));

                inventarioProducto = new InventarioProducto
                {
                    Id = Guid.NewGuid(),
                    InventarioId = inventario.Id,
                    ProductoId = dto.ProductoId,
                    StockActual = 0
                };

                _context.InventarioProductos.Add(inventarioProducto);
            }

            // =========================
            // 4️⃣ Movimiento inventario
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
                Observaciones = "Distribución desde contenedor a almacén"
            };

            inventarioProducto.StockActual = stockPosterior;

            _context.MovimientosInventario.Add(movimiento);

            // =========================
            // 5️⃣ Guardar todo
            // =========================
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Distribución realizada correctamente"));
        }
    }
}
