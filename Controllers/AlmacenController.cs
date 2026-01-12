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
    public class AlmacenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlmacenController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AlmacenResponseDto>>>> GetAlmacenes()
        {
            var almacenes = await _context.Almacenes
                .Include(a => a.inventario)
                .ToListAsync();

            var result = almacenes.Select(a => new AlmacenResponseDto
            {
                Id = a.id,
                Nombre = a.nombre,
                NombreEncargado = a.nombreencargado,
                Ubicacion = a.Ubicacion,
                Descripcion = a.Descripcion,
                InventarioId = a.inventario.Id
            }).ToList();

            return Ok(ApiResponse<IEnumerable<AlmacenResponseDto>>.Ok(
                result,
                "Almacenes obtenidos correctamente"
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AlmacenResponseDto>>> GetAlmacenById(Guid id)
        {
            var almacen = await _context.Almacenes
                .Include(a => a.inventario)
                .FirstOrDefaultAsync(a => a.id == id);

            if (almacen == null)
                return NotFound(ApiResponse<AlmacenResponseDto>.Fail($"Almacén con id {id} no encontrado", "ALMACEN_NOT_FOUND"));

            var result = new AlmacenResponseDto
            {
                Id = almacen.id,
                Nombre = almacen.nombre,
                NombreEncargado = almacen.nombreencargado,
                Ubicacion = almacen.Ubicacion,
                Descripcion = almacen.Descripcion,
                InventarioId = almacen.inventario.Id
            };

            return Ok(ApiResponse<AlmacenResponseDto>.Ok(result, "Almacén obtenido correctamente"));
        }



        [HttpPost]
        public async Task<ActionResult<ApiResponse<AlmacenResponseDto>>> CreateAlmacen([FromBody] AlmacenCreateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<AlmacenResponseDto>.Fail("El cuerpo de la petición es obligatorio", "INVALID_BODY"));

            var almacen = new Almacen
            {
                id = Guid.NewGuid(),
                nombre = dto.Nombre,
                nombreencargado = dto.NombreEncargado,
                Ubicacion = dto.Ubicacion,
                Descripcion = dto.Descripcion,
                inventario = new Inventario
                {
                    Id = Guid.NewGuid()
                }
            };

            almacen.inventario.AlmacenId = almacen.id;

            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            var result = new AlmacenResponseDto
            {
                Id = almacen.id,
                Nombre = almacen.nombre,
                NombreEncargado = almacen.nombreencargado,
                Ubicacion = almacen.Ubicacion,
                Descripcion = almacen.Descripcion,
                InventarioId = almacen.inventario.Id
            };

            return CreatedAtAction(
                nameof(GetAlmacenById),
                new { almacen.id },
                ApiResponse<AlmacenResponseDto>.Ok(result, "Almacén creado correctamente")
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateAlmacen(Guid id, [FromBody] AlmacenUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(ApiResponse<object>.Fail("El cuerpo de la petición es obligatorio", "INVALID_BODY"));

            var almacenDb = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.id == id);

            if (almacenDb == null)
                return NotFound(ApiResponse<object>.Fail($"Almacén con id {id} no encontrado", "ALMACEN_NOT_FOUND"));

            almacenDb.nombre = dto.Nombre;
            almacenDb.nombreencargado = dto.NombreEncargado;
            almacenDb.Ubicacion = dto.Ubicacion;
            almacenDb.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok("Almacén actualizado correctamente"));
        }


    }




}