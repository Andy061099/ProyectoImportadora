using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using ImportadoraApi.Responses;
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

        // =========================
        // GET: api/venta
        // =========================
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VentaResponseDto>>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Pagos)
                .ToListAsync();

            var response = ventas.Select(v => new VentaResponseDto
            {
                Id = v.Id,
                Fecha = v.Fecha,
                TipoVenta = v.TipoVenta,
                AlmacenId = v.AlmacenId,
                Cliente = v.Cliente,
                Total = v.Total,
                TotalPagado = v.TotalPagado,
                Usuario = v.UsuarioId,
                Estado = v.Estado, // <-- ahora es enum
                MonedaDeclarada = v.MonedaDeclarada,
                Detalles = v.Detalles.Select(d => new VentaDetalleResponseDto
                {
                    Id = d.Id,
                    InventarioProductoId = d.InventarioProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Impuestos = d.Impuestos,
                    Pagos = d.Pagos.Select(p => new PagoResponseDto
                    {
                        TipoMoneda = p.TipoMoneda,
                        Cantidad = p.Cantidad
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(ApiResponse<IEnumerable<VentaResponseDto>>.Ok(
                response,
                "Ventas obtenidas correctamente"
            ));
        }

        // =========================
        // GET: api/venta/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VentaResponseDto>>> GetVentaById(Guid id)
        {
            var venta = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Pagos)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound(ApiResponse<VentaResponseDto>.Fail(
                    $"Venta con id {id} no encontrada",
                    "VENTA_NOT_FOUND"
                ));

            var ventaDto = new VentaResponseDto
            {
                Id = venta.Id,
                Fecha = venta.Fecha,
                TipoVenta = venta.TipoVenta,
                AlmacenId = venta.AlmacenId,
                Cliente = venta.Cliente,
                Usuario = venta.UsuarioId,
                Total = venta.Total,
                TotalPagado = venta.TotalPagado,
                Estado = venta.Estado, // <-- enum
                MonedaDeclarada = venta.MonedaDeclarada,
                Detalles = venta.Detalles.Select(d => new VentaDetalleResponseDto
                {
                    InventarioProductoId = d.InventarioProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal,
                    Impuestos = d.Impuestos,
                    Pagos = d.Pagos.Select(p => new PagoResponseDto
                    {
                        Cantidad = p.Cantidad,
                        TipoMoneda = p.TipoMoneda
                    }).ToList()
                }).ToList()
            };

            return Ok(ApiResponse<VentaResponseDto>.Ok(
                ventaDto,
                "Venta obtenida correctamente"
            ));
        }

        // =========================
        // POST: api/venta
        // =========================
        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> PostVenta([FromBody] VentaCreateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<object>.Fail(
                    "El cuerpo de la venta es obligatorio",
                    "INVALID_BODY"
                ));

            if (dto.Detalles == null || !dto.Detalles.Any())
                return BadRequest(ApiResponse<object>.Fail(
                    "La venta debe tener al menos un producto",
                    "EMPTY_DETAILS"
                ));

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                decimal totalVenta = 0;

                var venta = new Venta
                {
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.UtcNow,
                    TipoVenta = dto.TipoVenta,
                    AlmacenId = dto.AlmacenId,
                    Cliente = dto.Cliente,
                    UsuarioId = dto.UsuarioId,
                    MonedaDeclarada = dto.MonedaDeclarada
                };

                _context.Ventas.Add(venta);

                foreach (var d in dto.Detalles)
                {
                    if (d.Cantidad <= 0)
                        return BadRequest(ApiResponse<object>.Fail(
                            "Cantidad inválida",
                            "INVALID_QUANTITY"
                        ));

                    if (d.PrecioUnitario <= 0)
                        return BadRequest(ApiResponse<object>.Fail(
                            "Precio unitario inválido",
                            "INVALID_PRICE"
                        ));

                    var inventarioProducto = await _context.InventarioProductos
                        .FirstOrDefaultAsync(ip => ip.Id == d.InventarioProductoId);

                    if (inventarioProducto == null)
                        return BadRequest(ApiResponse<object>.Fail(
                            "Producto no encontrado en inventario",
                            "INVENTORY_PRODUCT_NOT_FOUND"
                        ));

                    if (inventarioProducto.StockActual < d.Cantidad)
                        return BadRequest(ApiResponse<object>.Fail(
                            "Stock insuficiente",
                            "INSUFFICIENT_STOCK"
                        ));

                    var subtotal = d.Cantidad * d.PrecioUnitario;
                    var totalDetalle = subtotal + d.Impuestos;
                    totalVenta += totalDetalle;

                    var detalle = new VentaDetalle
                    {
                        Id = Guid.NewGuid(),
                        VentaId = venta.Id,
                        InventarioProductoId = inventarioProducto.Id,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = subtotal,
                        Impuestos = d.Impuestos
                    };

                    _context.VentaDetalles.Add(detalle);

                    // PAGOS POR MONEDA
                    if (d.Pagos == null || !d.Pagos.Any())
                        return BadRequest(ApiResponse<object>.Fail(
                            "Cada detalle debe tener al menos un pago",
                            "PAYMENTS_REQUIRED"
                        ));

                    foreach (var p in d.Pagos)
                    {
                        if (p.Cantidad <= 0)
                            return BadRequest(ApiResponse<object>.Fail(
                                "Cantidad de pago inválida",
                                "INVALID_PAYMENT"
                            ));

                        var pago = new Pagos
                        {
                            Id = Guid.NewGuid(),
                            VentaDetalleId = detalle.Id,
                            Cantidad = p.Cantidad,
                            TipoMoneda = p.TipoMoneda
                        };

                        _context.Pagos.Add(pago);
                    }

                    // MOVIMIENTO DE INVENTARIO
                    var stockAnterior = inventarioProducto.StockActual;
                    var stockPosterior = stockAnterior - d.Cantidad;

                    inventarioProducto.StockActual = stockPosterior;

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
                        Observaciones = "Venta",
                        OrSigen = Origen.Venta,
                        UsuarioId = venta.UsuarioId
                    };

                    _context.MovimientosInventario.Add(movimiento);
                }

                // TOTALES DE VENTA
                venta.Total = totalVenta;
                venta.TotalPagado = dto.TotalPagado;
                venta.Estado = dto.TotalPagado < totalVenta ? EstadoVenta.Pagada : EstadoVenta.Pagada;

                // CONSIGNACIÓN (SI APLICA)
                if (venta.TotalPagado < venta.Total)
                {
                    var consignacion = new Consignacion
                    {
                        Id = Guid.NewGuid(),
                        VentaId = venta.Id,
                        MontoTotal = venta.Total,
                        MontoPendiente = venta.Total - venta.TotalPagado,
                        Estado = EstadoConsigacion.ABIERTA
                    };
                    _context.Consignaciones.Add(consignacion);
                }

                // REGISTRO FINANCIERO
                var registroFinanciero = new RegistroFinanciero
                {
                    Id = Guid.NewGuid(),
                    Fecha = DateTime.UtcNow,
                    Tipo = TipoRegistroFinanciero.Ingreso,
                    AlmacenId = venta.AlmacenId,
                    Monto = venta.TotalPagado,
                    ReferenciaTipo = "Venta",
                    ReferenciaId = venta.Id,
                    MonedaDeclarada = venta.MonedaDeclarada,
                    UsuarioId = venta.UsuarioId,
                    Observaciones = "Registro automático por venta"
                };
                _context.RegistrosFinancieros.Add(registroFinanciero);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Ok(
                    new { venta.Id },
                    "Venta registrada correctamente"
                ));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, ApiResponse<object>.Fail(
                    "Error interno al registrar la venta",
                    "INTERNAL_ERROR"
                ));
            }
        }
    }
}
