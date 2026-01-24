using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
using ImportadoraApi.Responses;
using System.Security.Claims;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MermaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MermaController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/merma
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Mermas
                .Include(m => m.Producto)
                .Include(m => m.Almacen)
                .AsNoTracking()
                .Select(m => new MermaResponseDto
                {
                    MermaId = m.Id,
                    Producto = m.Producto.Nombre,
                    Almacen = m.Almacen.nombre,
                    Cantidad = m.Cantidad,
                    Fecha = m.Fecha,
                    Motivo = m.Motivo,

                })
                .ToListAsync();

            return Ok(ApiResponse<List<MermaResponseDto>>
                .Ok(data, "Mermas obtenidas correctamente"));
        }

        // =========================
        // GET: api/merma/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.Mermas
                .Include(m => m.Producto)
                .Include(m => m.Almacen)
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new MermaResponseDto
                {
                    MermaId = m.Id,
                    Producto = m.Producto.Nombre,
                    Almacen = m.Almacen.nombre,
                    Cantidad = m.Cantidad,
                    Fecha = m.Fecha,
                    Motivo = m.Motivo,

                })
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound(ApiResponse<string>.Fail(
                    "Merma no encontrada",
                    "MERMA_404"
                ));

            return Ok(ApiResponse<MermaResponseDto>
                .Ok(data, "Merma obtenida correctamente"));
        }

        // =========================
        // POST: api/merma
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MermaCreateDto merma)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail(
                    "Datos inválidos",
                    "MERMA_001"
                ));

            var inventarioProducto = await _context.InventarioProductos
                .Include(ip => ip.Inventario)
                    .ThenInclude(i => i.Almacen)
                .Include(ip => ip.Producto)
                .FirstOrDefaultAsync(ip =>
                    ip.ProductoId == merma.ProductoId &&
                    ip.Inventario.AlmacenId == merma.AlmacenId);

            if (inventarioProducto == null)
                return BadRequest(ApiResponse<string>.Fail(
                    "El producto no existe en este almacén",
                    "MERMA_002"
                ));

            var disponible = inventarioProducto.StockActual;
            if (merma.Cantidad > disponible)
                return BadRequest(ApiResponse<string>.Fail(
                    "La cantidad supera el stock disponible",
                    "MERMA_003"
                ));

            // Usar la misma fecha para todo
            var fechaAhora = DateTime.UtcNow;
            var mermaId = Guid.NewGuid();
            var stockAnterior = inventarioProducto.StockActual;
            var stockPosterior = stockAnterior - merma.Cantidad;

            // =========================
            // Movimiento de inventario
            // =========================
            var movimiento = new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                InventarioProductoId = inventarioProducto.Id,
                Cantidad = merma.Cantidad,
                StockAnterior = stockAnterior,
                StockPosterior = stockPosterior,
                TipoMovimiento = TipoMovimientoInventario.Salida,
                OrSigen = Origen.Merma,
                ReferenciaId = mermaId,
                Observaciones = merma.Motivo,
                UsuarioId = merma.UsuarioId,
                Fecha = fechaAhora
            };
            inventarioProducto.StockActual = stockPosterior;
            _context.MovimientosInventario.Add(movimiento);

            // =========================
            // Registro financiero
            // =========================
            _context.RegistrosFinancieros.Add(new RegistroFinanciero
            {
                Id = Guid.NewGuid(),
                AlmacenId = inventarioProducto.Inventario.AlmacenId,
                ReferenciaId = mermaId,
                ReferenciaTipo = "Merma de producto",
                Observaciones = merma.Motivo,
                Tipo = TipoRegistroFinanciero.Gasto,
                Monto = inventarioProducto.Producto.CostoUnitario * merma.Cantidad,
                MonedaDeclarada = inventarioProducto.Producto.MonedaDeEntrada,
                Fecha = fechaAhora,
                UsuarioId = merma.UsuarioId
            });

            // =========================
            // Entidad Merma
            // =========================
            var mermaEntity = new Merma
            {
                Id = mermaId,
                ProductoId = merma.ProductoId,
                AlmacenId = merma.AlmacenId,
                Cantidad = merma.Cantidad,
                Motivo = merma.Motivo,
                UsuarioId = merma.UsuarioId,
                Fecha = fechaAhora,

            };
            _context.Mermas.Add(mermaEntity);

            await _context.SaveChangesAsync();

            // =========================
            // DTO de respuesta
            // =========================
            var response = new MermaResponseDto
            {
                MermaId = mermaId,
                Producto = inventarioProducto.Producto.Nombre,
                Almacen = inventarioProducto.Inventario.Almacen!.nombre,
                Cantidad = merma.Cantidad,
                Fecha = fechaAhora,
                Motivo = merma.Motivo,
                StockAnterior = stockAnterior,
                StockPosterior = stockPosterior
            };

            return Ok(ApiResponse<MermaResponseDto>
                .Ok(response, "Merma registrada correctamente"));
        }
    }
}
