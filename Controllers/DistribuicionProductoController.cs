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
                .AsNoTracking()
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

            return Ok(ApiResponse<List<DistribucionProductoResponseDto>>.Ok(distribuciones, "Distribuciones obtenidas correctamente"));
        }

        // =========================
        // GET: api/distribucionproducto/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistribucion(Guid id)
        {
            var distribucion = await _context.DistribucionProductos
                .AsNoTracking()
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
                return NotFound(ApiResponse<DistribucionProductoResponseDto>.Fail("Distribución no encontrada", "DISTRIBUCION_NOT_FOUND"));

            return Ok(ApiResponse<DistribucionProductoResponseDto>.Ok(distribucion, "Distribución obtenida correctamente"));
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

            if (dto.Cantidad <= 0)
                return BadRequest(ApiResponse<string>.Fail("La cantidad debe ser mayor que cero"));

            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Distribuciones)
                .FirstOrDefaultAsync(d => d.Id == dto.OrigenId);

            var almacen = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.id == dto.AlmacenId);

            if (almacen == null)
                return BadRequest(ApiResponse<string>.Fail("Almacén no encontrado"));
            if (detalle == null)
                return BadRequest(ApiResponse<string>.Fail("Detalle de contenedor no encontrado"));
            if (detalle.ProductoId != dto.ProductoId)
                return BadRequest(ApiResponse<string>.Fail("El producto no coincide con el detalle de origen"));

            var yaDistribuido = detalle.Distribuciones.Sum(d => d.Cantidad);
            var disponible = detalle.CantidadRecibida - yaDistribuido;

            if (dto.Cantidad > disponible)
                return BadRequest(ApiResponse<string>.Fail("La cantidad supera lo disponible en el contenedor"));

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(ApiResponse<string>.Fail("Usuario no autenticado"));

            var userId = Guid.Parse(userIdClaim);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // =========================
                // Crear distribución
                // =========================
                var distribucion = new DistribucionProducto
                {
                    Id = Guid.NewGuid(),
                    OrigenId = dto.OrigenId,
                    ProductoId = dto.ProductoId,
                    AlmacenId = dto.AlmacenId,
                    Cantidad = dto.Cantidad,
                    Fecha = DateTime.UtcNow,
                    UsuarioId = userId
                };
                _context.DistribucionProductos.Add(distribucion);

                // =========================
                // InventarioProducto
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
                // Movimiento inventario
                // =========================
                var stockAnterior = inventarioProducto.StockActual;
                inventarioProducto.StockActual += dto.Cantidad;

                var movimiento = new MovimientoInventario
                {
                    Id = Guid.NewGuid(),
                    InventarioProductoId = inventarioProducto.Id,
                    TipoMovimiento = TipoMovimientoInventario.Entrada,
                    Fecha = DateTime.UtcNow,
                    Cantidad = dto.Cantidad,
                    StockAnterior = stockAnterior,
                    StockPosterior = inventarioProducto.StockActual,
                    ReferenciaId = dto.OrigenId,
                    Observaciones = "Distribución desde contenedor a almacén"
                };
                _context.MovimientosInventario.Add(movimiento);

                // =========================
                // Guardar todo
                // =========================
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // =========================
                // Preparar DTO de respuesta
                // =========================
                var response = new DistribucionProductoResponseDto
                {
                    Id = distribucion.Id,
                    ProductoId = distribucion.ProductoId,
                    NombreProducto = (await _context.Productos.FindAsync(distribucion.ProductoId))?.Nombre ?? "",
                    AlmacenId = distribucion.AlmacenId,
                    NombreAlmacen = almacen.nombre,
                    Cantidad = distribucion.Cantidad,
                    Fecha = distribucion.Fecha
                };

                return Ok(ApiResponse<DistribucionProductoResponseDto>.Ok(response, "Distribución realizada correctamente"));
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<string>.Fail("Error interno al registrar la distribución", "INTERNAL_ERROR"));
            }
        }
    }
}
