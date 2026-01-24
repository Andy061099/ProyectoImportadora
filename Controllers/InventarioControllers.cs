using ImportadoraApi.Models;
using ImportadoraApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventarioController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/inventario
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var inventarios = await _context.Inventarios
                .AsNoTracking()
                .Include(i => i.Almacen)
                .Include(i => i.Productos)
                    .ThenInclude(ip => ip.Producto)
                .Select(i => new InventarioResponseDto
                {
                    Id = i.Id,
                    AlmacenId = i.AlmacenId,
                    NombreAlmacen = i.Almacen!.nombre,
                    FechaCreacion = i.FechaCreacion,
                    Productos = i.Productos.Select(p => new InventarioProductoResponseDto
                    {
                        ProductoId = p.ProductoId,
                        NombreProducto = p.Producto.Nombre,
                        StockActual = p.StockActual
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ApiResponse<List<InventarioResponseDto>>.Ok(inventarios, "Inventarios obtenidos correctamente"));
        }

        // =========================
        // GET: api/inventario/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var inventario = await _context.Inventarios
                .AsNoTracking()
                .Include(i => i.Almacen)
                .Include(i => i.Productos)
                    .ThenInclude(ip => ip.Producto)
                .Where(i => i.Id == id)
                .Select(i => new InventarioResponseDto
                {
                    Id = i.Id,
                    AlmacenId = i.AlmacenId,
                    NombreAlmacen = i.Almacen!.nombre,
                    FechaCreacion = i.FechaCreacion,
                    Productos = i.Productos.Select(p => new InventarioProductoResponseDto
                    {
                        ProductoId = p.ProductoId,
                        NombreProducto = p.Producto.Nombre,
                        StockActual = p.StockActual
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (inventario == null)
                return NotFound(ApiResponse<InventarioResponseDto>.Fail("Inventario no encontrado", "INVENTARIO_NOT_FOUND"));

            return Ok(ApiResponse<InventarioResponseDto>.Ok(inventario, "Inventario obtenido correctamente"));
        }
    }
}
