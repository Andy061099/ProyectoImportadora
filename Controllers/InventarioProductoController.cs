using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Authorization;
using ImportadoraApi.Responses;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventarioProductoController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/InventarioProducto
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.InventarioProductos
                .AsNoTracking()
                .Include(ip => ip.Producto)
                .Select(ip => new InventarioProductoResponseDto
                {
                    ProductoId = ip.ProductoId,
                    NombreProducto = ip.Producto.Nombre,
                    StockActual = ip.StockActual,
                    Packa = ip.Packa,
                    CantidadPorPacka = ip.Packa ? ip.Cantproductosxpacka : 0,
                    TotalPackas = ip.Packa && ip.Cantproductosxpacka > 0
                        ? ip.StockActual / ip.Cantproductosxpacka
                        : 0
                })
                .ToListAsync();

            return Ok(ApiResponse<List<InventarioProductoResponseDto>>.Ok(
                data,
                "Inventario obtenido correctamente"
            ));
        }

        // =========================
        // GET: api/InventarioProducto/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _context.InventarioProductos
                .AsNoTracking()
                .Include(ip => ip.Producto)
                .Where(ip => ip.Id == id)
                .Select(ip => new InventarioProductoResponseDto
                {
                    ProductoId = ip.ProductoId,
                    NombreProducto = ip.Producto.Nombre,
                    StockActual = ip.StockActual,
                    Packa = ip.Packa,
                    CantidadPorPacka = ip.Packa ? ip.Cantproductosxpacka : 0,
                    TotalPackas = ip.Packa && ip.Cantproductosxpacka > 0
                        ? ip.StockActual / ip.Cantproductosxpacka
                        : 0
                })
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound(ApiResponse<InventarioProductoResponseDto>.Fail(
                    "Producto de inventario no encontrado",
                    "INV_404"
                ));

            return Ok(ApiResponse<InventarioProductoResponseDto>.Ok(
                data,
                "Producto de inventario obtenido correctamente"
            ));
        }

        // =========================
        // PUT: api/InventarioProducto/{id}
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] InventarioProductoUpdateDto dto)
        {
            var inventarioProductoDb = await _context.InventarioProductos
                .FirstOrDefaultAsync(ip => ip.Id == id);

            if (inventarioProductoDb == null)
                return NotFound(ApiResponse<InventarioProductoResponseDto>.Fail(
                    "Producto de inventario no encontrado",
                    "INV_404"
                ));

            // 🔐 Reglas de negocio
            if (!dto.Packa)
            {
                inventarioProductoDb.Packa = false;
                inventarioProductoDb.Cantproductosxpacka = 0;
            }
            else
            {
                if (dto.Cantproductosxpacka <= 0)
                    return BadRequest(ApiResponse<InventarioProductoResponseDto>.Fail(
                        "Cantidad por packa inválida",
                        "INV_002"
                    ));

                inventarioProductoDb.Packa = true;
                inventarioProductoDb.Cantproductosxpacka = dto.Cantproductosxpacka;
            }

            await _context.SaveChangesAsync();

            // devolver DTO actualizado
            var responseDto = new InventarioProductoResponseDto
            {
                ProductoId = inventarioProductoDb.ProductoId,
                NombreProducto = inventarioProductoDb.Producto?.Nombre ?? "",
                StockActual = inventarioProductoDb.StockActual,
                Packa = inventarioProductoDb.Packa,
                CantidadPorPacka = inventarioProductoDb.Packa ? inventarioProductoDb.Cantproductosxpacka : 0,
                TotalPackas = inventarioProductoDb.Packa && inventarioProductoDb.Cantproductosxpacka > 0
                    ? inventarioProductoDb.StockActual / inventarioProductoDb.Cantproductosxpacka
                    : 0
            };

            return Ok(ApiResponse<InventarioProductoResponseDto>.Ok(
                responseDto,
                "Producto de inventario actualizado correctamente"
            ));
        }
    }


}
