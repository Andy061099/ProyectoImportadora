using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;

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
        // GET: api/producto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Merma>>> GetMerma()
        {
            var mermas = await _context.Mermas
                .AsNoTracking()
                .ToListAsync();

            return Ok(mermas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Merma>> GetProductoById(Guid id)
        {
            var producto = await _context.Mermas
            .Include(c => c.Producto)
            .Include(C => C.Almacen)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
                return NotFound($"Producto con id {id} no encontrado");

            return Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMerma([FromBody] Merma merma)
        {

            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            // Validación básica
            if (merma == null)
                return BadRequest("El *** es obligatorio.");


            var productoinventario = await _context.InventarioProductos
            .FirstOrDefaultAsync(c => c.ProductoId == merma.ProductoId &&
            c.Inventario.AlmacenId == merma.AlmacenId);


            if (productoinventario == null)
                return BadRequest("Este producto no existe en este almacen");


            if (productoinventario.StockActual < merma.Cantidad)
            {
                return BadRequest("la cantidad que quiere dar baja por merma supera la cantidad real");
            }

            merma.Id = Guid.NewGuid();


            var movimiento = new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                Cantidad = merma.Cantidad,
                InventarioProductoId = productoinventario.Id,
                InventarioProducto = productoinventario,
                ReferenciaId = merma.Id,
                StockAnterior = productoinventario.StockActual,
                StockPosterior = productoinventario.StockActual = -merma.Cantidad,
                TipoMovimiento = TipoMovimientoInventario.Salida,
                OrSigen = Origen.Merma,
                Observaciones = merma.Motivo,

            };

            _context.RegistrosFinancieros.Add(new RegistroFinanciero
            {
                Id = Guid.NewGuid(),
                AlmacenId = productoinventario.Inventario.AlmacenId,
                Almacen = productoinventario.Inventario.Almacen,
                ReferenciaId = merma.Id,
                Observaciones = merma.Motivo,
                Tipo = TipoRegistroFinanciero.Gasto,
                Monto = productoinventario.Producto.CostoUnitario * merma.Cantidad,
                Moneda = productoinventario.Producto.MonedaDeEntrada,
                ReferenciaTipo = "Producto sacado del almacén por Merma",


            });
            productoinventario.StockActual = movimiento.StockPosterior;
            _context.Mermas.Add(merma);
            await _context.SaveChangesAsync();

            return Ok(movimiento);



        }






    }
}