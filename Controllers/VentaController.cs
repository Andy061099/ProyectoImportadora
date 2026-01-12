using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentaController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVenta()
        {
            var Venta = await _context.Ventas
                .AsNoTracking()
                .ToListAsync();

            return Ok(Venta);
        }

        // GET: api/Venta/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVentaById(Guid id)
        {
            var Venta = await _context.Ventas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (Venta == null)
                return NotFound($"Producto con id {id} no encontrado");

            return Ok(Venta);
        }

        [HttpPost]
        public async Task<IActionResult> PostVenta([FromBody] VentaCreateDto dto)
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                return BadRequest("La venta debe tener al menos un producto.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // =========================
                // 1️⃣ CALCULAR TOTAL REAL
                // =========================
                decimal total = 0;

                foreach (var d in dto.Detalles)
                {
                    if (d.Cantidad <= 0)
                        return BadRequest("Cantidad inválida.");

                    if (d.PrecioUnitario <= 0)
                        return BadRequest("Precio inválido.");

                    total += d.Cantidad * d.PrecioUnitario;
                }

                if (dto.TotalPagado < 0 || dto.TotalPagado > total)
                    return BadRequest("Total pagado inválido.");

                // =========================
                // 2️⃣ CREAR VENTA
                // =========================
                var venta = new Venta
                {
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.UtcNow,
                    TipoVenta = dto.TipoVenta, // Mayorista / Minorista
                    AlmacenId = dto.AlmacenId,
                    Cliente = dto.Cliente,
                    Total = total,
                    TotalPagado = dto.TotalPagado,
                    UsuarioId = dto.UsuarioId,
                    Estado = dto.TotalPagado < total ? "PENDIENTE" : "PAGADA"
                };

                _context.Ventas.Add(venta);

                // =========================
                // 3️⃣ PROCESAR DETALLES + INVENTARIO
                // =========================
                foreach (var d in dto.Detalles)
                {
                    var inventarioProducto = await _context.InventarioProductos
                        .FirstOrDefaultAsync(ip => ip.Id == d.InventarioProductoId);

                    if (inventarioProducto == null)
                        return BadRequest("Producto no encontrado en inventario.");

                    if (inventarioProducto.StockActual < d.Cantidad)
                        return BadRequest("Stock insuficiente.");

                    var stockAnterior = inventarioProducto.StockActual;
                    var stockPosterior = stockAnterior - d.Cantidad;

                    var detalle = new VentaDetalle
                    {
                        Id = Guid.NewGuid(),
                        VentaId = venta.Id,
                        InventarioProductoId = d.InventarioProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario,
                        Impuestos = 0
                    };

                    _context.VentaDetalles.Add(detalle);

                    var movimiento = new MovimientoInventario
                    {
                        Id = Guid.NewGuid(),
                        InventarioProductoId = inventarioProducto.Id,
                        TipoMovimiento = TipoMovimientoInventario.Salida,
                        Fecha = DateTime.UtcNow,
                        Cantidad = d.Cantidad,
                        StockAnterior = stockAnterior,
                        StockPosterior = stockPosterior,
                        ReferenciaId = venta.Id,
                        Observaciones = "Venta"
                    };

                    _context.MovimientosInventario.Add(movimiento);

                    inventarioProducto.StockActual = stockPosterior;
                }

                // =========================
                // 4️⃣ CREAR CONSIGNACIÓN SI HAY DEUDA
                // =========================
                if (venta.TotalPagado < venta.Total)
                {
                    var pendiente = venta.Total - venta.TotalPagado;

                    var consignacion = new Consignacion
                    {
                        Id = Guid.NewGuid(),
                        VentaId = venta.Id,
                        MontoTotal = venta.Total,
                        MontoPendiente = pendiente,
                        Estado = "ABIERTA"
                    };

                    _context.Consignaciones.Add(consignacion);

                    // Si pagó algo, registrar movimiento
                    if (venta.TotalPagado > 0)
                    {
                        var mov = new ConsignacionMovimiento
                        {
                            Id = Guid.NewGuid(),
                            ConsignacionId = consignacion.Id,
                            Fecha = DateTime.UtcNow,
                            Monto = venta.TotalPagado,
                            Observaciones = "Pago inicial",
                            UsuarioId = venta.UsuarioId
                        };

                        _context.ConsignacionMovimientos.Add(mov);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ex.Message);
            }
        }





    }
}