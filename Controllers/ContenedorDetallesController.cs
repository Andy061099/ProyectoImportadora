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
    public class ContenedorDetallesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContenedorDetallesController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/contenedordetalles
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetDetalles()
        {
            var detalles = await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Include(d => d.Contenedor)
                .OrderByDescending(d => d.Contenedor.FechaArribo)
                .Select(d => new ContenedorDetalleResponseDto
                {
                    Id = d.Id,
                    ContenedorId = d.ContenedorId,
                    CodigoContenedor = d.Contenedor.Codigo,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    CantidadRecibida = d.CantidadRecibida,
                    CantidadActual = d.Cantidadactual,
                    Packa = d.Packa,
                    CantProductosPorPacka = d.Cantproductosxpacka,
                    CantidadMerma = d.Cantidadmerma,
                    CostoUnitario = d.CostoUnitario
                })
                .ToListAsync();

            return Ok(ApiResponse<List<ContenedorDetalleResponseDto>>.Ok(detalles));
        }

        // =========================
        // GET: api/contenedordetalles/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetalle(Guid id)
        {
            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Include(d => d.Contenedor)
                .Where(d => d.Id == id)
                .Select(d => new ContenedorDetalleResponseDto
                {
                    Id = d.Id,
                    ContenedorId = d.ContenedorId,
                    CodigoContenedor = d.Contenedor.Codigo,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    CantidadRecibida = d.CantidadRecibida,
                    CantidadActual = d.Cantidadactual,
                    Packa = d.Packa,
                    CantProductosPorPacka = d.Cantproductosxpacka,
                    CantidadMerma = d.Cantidadmerma,
                    CostoUnitario = d.CostoUnitario
                })
                .FirstOrDefaultAsync();

            if (detalle == null)
                return NotFound(ApiResponse<string>.Fail("Detalle no encontrado"));

            return Ok(ApiResponse<ContenedorDetalleResponseDto>.Ok(detalle));
        }

        // =========================
        // GET: api/contenedordetalles/contenedor/{contenedorId}
        // =========================
        [HttpGet("contenedor/{contenedorId}")]
        public async Task<IActionResult> GetDetallesPorContenedor(Guid contenedorId)
        {
            var detalles = await _context.ContenedorDetalles
                .Include(d => d.Producto)
                .Where(d => d.ContenedorId == contenedorId)
                .Select(d => new ContenedorDetalleResponseDto
                {
                    Id = d.Id,
                    ContenedorId = d.ContenedorId,
                    CodigoContenedor = d.Contenedor.Codigo,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre,
                    CantidadRecibida = d.CantidadRecibida,
                    CantidadActual = d.Cantidadactual,
                    Packa = d.Packa,
                    CantProductosPorPacka = d.Cantproductosxpacka,
                    CantidadMerma = d.Cantidadmerma,
                    CostoUnitario = d.CostoUnitario
                })
                .ToListAsync();

            return Ok(ApiResponse<List<ContenedorDetalleResponseDto>>.Ok(detalles));
        }

        // =========================
        // POST: api/contenedordetalles
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDetalle([FromBody] PostContenedorDetalleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Datos inválidos"));

            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == dto.ContenedorId);

            if (contenedor == null)
                return BadRequest(ApiResponse<string>.Fail("El contenedor no existe"));

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == dto.ProductoId);

            if (producto == null)
                return BadRequest(ApiResponse<string>.Fail("El producto no existe"));

            var detalle = new ContenedorDetalle
            {
                Id = Guid.NewGuid(),
                ContenedorId = dto.ContenedorId,
                ProductoId = dto.ProductoId,
                CantidadRecibida = dto.CantidadRecibida,
                Cantidadactual = dto.CantidadRecibida,
                Packa = dto.Packa,
                Cantproductosxpacka = dto.CantProductosPorPacka,
                Cantidadmerma = dto.CantidadMerma,
                CostoUnitario = dto.CostoUnitario
            };

            _context.ContenedorDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Detalle agregado correctamente"));
        }

        // =========================
        // PUT: api/contenedordetalles/{id}
        // =========================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetalle(Guid id, [FromBody] PostContenedorDetalleDto dto)
        {
            var detalle = await _context.ContenedorDetalles
                .Include(d => d.Distribuciones)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (detalle == null)
                return NotFound(ApiResponse<string>.Fail("Detalle no encontrado"));

            if (detalle.Distribuciones.Any())
                return BadRequest(ApiResponse<string>.Fail("No se puede modificar un detalle ya distribuido"));

            detalle.CantidadRecibida = dto.CantidadRecibida;
            detalle.Cantidadactual = dto.CantidadRecibida;
            detalle.Packa = dto.Packa;
            detalle.Cantproductosxpacka = dto.CantProductosPorPacka;
            detalle.Cantidadmerma = dto.CantidadMerma;
            detalle.CostoUnitario = dto.CostoUnitario;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("OK", "Detalle actualizado correctamente"));
        }
    }
}
