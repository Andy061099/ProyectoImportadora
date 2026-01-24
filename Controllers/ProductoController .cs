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
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: api/producto
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .ToListAsync();

            var result = productos.Select(p => new ProductoResponseDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                UnidadMedida = p.UnidadMedida,
                CostoUnitario = p.CostoUnitario,
                PrecioMayorista = p.PrecioMayorista,
                PrecioMinorista = p.PrecioMinorista,
                MonedaDeEntrada = p.MonedaDeEntrada
            }).ToList();

            return Ok(ApiResponse<List<ProductoResponseDto>>.Ok(
                result,
                "Productos obtenidos correctamente"
            ));
        }

        // =========================
        // GET: api/producto/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(Guid id)
        {
            var p = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null)
                return NotFound(ApiResponse<string>.Fail(
                    $"Producto con id {id} no encontrado",
                    "PRODUCTO_404"
                ));

            var result = new ProductoResponseDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                UnidadMedida = p.UnidadMedida,
                CostoUnitario = p.CostoUnitario,
                PrecioMayorista = p.PrecioMayorista,
                PrecioMinorista = p.PrecioMinorista,
                MonedaDeEntrada = p.MonedaDeEntrada
            };

            return Ok(ApiResponse<ProductoResponseDto>.Ok(
                result,
                "Producto obtenido correctamente"
            ));
        }

        // =========================
        // POST: api/producto
        // =========================
        [HttpPost]
        public async Task<IActionResult> CreateProducto([FromBody] ProductoCreateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail(
                    "El cuerpo de la petición es obligatorio",
                    "INVALID_BODY"
                ));

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(ApiResponse<string>.Fail(
                    "El nombre es obligatorio",
                    "INVALID_NAME"
                ));

            if (string.IsNullOrWhiteSpace(dto.Codigo))
                return BadRequest(ApiResponse<string>.Fail(
                    "El código es obligatorio",
                    "INVALID_CODE"
                ));

            var existeCodigo = await _context.Productos
                .AnyAsync(p => p.Codigo == dto.Codigo);

            if (existeCodigo)
                return Conflict(ApiResponse<string>.Fail(
                    "Ya existe un producto con ese código",
                    "DUPLICATE_CODE"
                ));

            var producto = new Producto
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                UnidadMedida = dto.UnidadMedida,
                CostoUnitario = dto.CostoUnitario,
                PrecioMayorista = dto.PrecioMayorista,
                PrecioMinorista = dto.PrecioMinorista,
                MonedaDeEntrada = dto.MonedaDeEntrada
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var result = new ProductoResponseDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                UnidadMedida = producto.UnidadMedida,
                CostoUnitario = producto.CostoUnitario,
                PrecioMayorista = producto.PrecioMayorista,
                PrecioMinorista = producto.PrecioMinorista,
                MonedaDeEntrada = producto.MonedaDeEntrada
            };

            return CreatedAtAction(
                nameof(GetProductoById),
                new { id = producto.Id },
                ApiResponse<ProductoResponseDto>.Ok(
                    result,
                    "Producto creado correctamente"
                )
            );
        }

        // =========================
        // PUT: api/producto/{id}
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(Guid id, [FromBody] ProductoUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<string>.Fail(
                    "El cuerpo de la petición es obligatorio",
                    "INVALID_BODY"
                ));

            var productoDb = await _context.Productos
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoDb == null)
                return NotFound(ApiResponse<string>.Fail(
                    $"Producto con id {id} no encontrado",
                    "PRODUCTO_404"
                ));

            // Código (actualizable)
            if (!string.IsNullOrWhiteSpace(dto.Codigo))
            {
                var existeCodigo = await _context.Productos
                    .AnyAsync(p => p.Codigo == dto.Codigo && p.Id != id);

                if (existeCodigo)
                    return Conflict(ApiResponse<string>.Fail(
                        "Ya existe un producto con ese código",
                        "DUPLICATE_CODE"
                    ));

                productoDb.Codigo = dto.Codigo;
            }

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                productoDb.Nombre = dto.Nombre;

            if (dto.Descripcion != null)
                productoDb.Descripcion = dto.Descripcion;

            if (!string.IsNullOrWhiteSpace(dto.UnidadMedida))
                productoDb.UnidadMedida = dto.UnidadMedida;

            if (dto.CostoUnitario.HasValue)
                productoDb.CostoUnitario = dto.CostoUnitario.Value;

            if (dto.PrecioMayorista.HasValue)
                productoDb.PrecioMayorista = dto.PrecioMayorista.Value;

            if (dto.PrecioMinorista.HasValue)
                productoDb.PrecioMinorista = dto.PrecioMinorista.Value;

            if (dto.MonedaDeEntrada.HasValue)
                productoDb.MonedaDeEntrada = dto.MonedaDeEntrada.Value;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok(
                "Producto actualizado correctamente"
            ));
        }
    }
}
