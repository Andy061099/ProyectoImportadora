using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Authorization;
namespace ImportadoraApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContenedorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContenedorController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 GET: api/contenedor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contenedor>>> GetContenedores()
        {
            return await _context.Contenedores
                .OrderByDescending(c => c.FechaArribo)
                .ToListAsync();
        }

        // 🔹 GET: api/contenedor/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Contenedor>> GetContenedor(Guid id)
        {
            var contenedor = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contenedor == null)
                return NotFound("Contenedor no encontrado");

            return Ok(contenedor);
        }

        // 🔹 POST: api/contenedor
        [HttpPost]
        public async Task<IActionResult> CreateContenedor([FromBody] PostContenedor contenedor)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);





            if (contenedor == null)
                return BadRequest("El contenedor es obligatorio");

            var existeCodigo = await _context.Contenedores
                    .AnyAsync(c => c.Codigo == contenedor.Codigo);

            if (existeCodigo)
                return Conflict("Ya existe un contenedor con ese código");

            Guid idContenedor = new Guid();
            Guid idRegistro = new Guid();
            _context.RegistrosFinancieros.Add(new RegistroFinanciero
            {
                Id = idRegistro,
                Moneda = contenedor.moneda,
                Monto = contenedor.CostoCompraContenedor,
                Observaciones = contenedor.Descripcion,
                ReferenciaTipo = "Compra Contenedor",
                ReferenciaId = idContenedor,
                Tipo = TipoRegistroFinanciero.Gasto,


            });

            var contenedoroficial = new Contenedor
            {
                Codigo = contenedor.Codigo,
                Id = idContenedor,
                NombreContenedor = contenedor.NombreContenedor,
                FechaArribo = contenedor.FechaArribo

            };

            _context.Contenedores.Add(contenedoroficial);
            await _context.SaveChangesAsync();

            return Ok(contenedoroficial);


        }
        // 🔹 PUT: api/contenedor/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContenedor(Guid id, [FromBody] Contenedor contenedor)
        {
            if (id != contenedor.Id)
                return BadRequest("El id no coincide");

            var contenedorDb = await _context.Contenedores
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contenedorDb == null)
                return NotFound("Contenedor no encontrado");


            if (contenedorDb.Codigo != contenedor.Codigo)
            {
                var Codigo = await _context.Contenedores.FirstOrDefaultAsync(c => c.Id != contenedorDb.Id && c.Codigo == contenedorDb.Codigo);
                if (Codigo != null)
                    return BadRequest("Ya existe un contenedor con ese código");

            }
            // 🔹 Solo datos operativos
            contenedorDb.FechaArribo = contenedor.FechaArribo;
            contenedorDb.Estado = contenedor.Estado;
            contenedorDb.NombreContenedor = contenedor.NombreContenedor;
            contenedorDb.Codigo = contenedor.Codigo;



            await _context.SaveChangesAsync();

            return Ok();
        }






    }
}