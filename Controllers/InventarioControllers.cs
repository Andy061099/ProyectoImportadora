using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImportadoraApi.Models;


namespace ImportadoraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/inventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventario>>> GetAll()
        {
            return await _context.Inventarios
                .Include(i => i.Productos)
                .ToListAsync();
        }

        // GET: api/inventario/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventario>> GetById(Guid id)
        {
            var inventario = await _context.Inventarios
                .Include(i => i.Productos)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventario == null)
                return NotFound();

            return inventario;
        }




    }
}