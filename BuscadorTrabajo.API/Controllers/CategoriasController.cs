using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoriasController(DataContext context)
        {
            _context = context;
        }

        // GET /api/categorias
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Categorias
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/categorias/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Categorias
                .AsNoTracking()
                .OrderBy(x => x.Id);

            var totalRecords = await query.CountAsync(); // Cuenta todos los registros

            var data = await query
                .Skip((page - 1) * pageSize) // Omite los registros de las páginas anteriores
                .Take(pageSize)              // Toma solo los de la página pedida
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return Ok(new
            {
                page,
                pageSize,
                totalRecords,
                totalPages,
                data
            });
        }

        // GET /api/categorias/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (categoria == null)
            {
                return NotFound("No existe una categoría con ese Id."); //404
            }

            return Ok(categoria);
        }

        // POST /api/categorias
        [HttpPost]
        public async Task<ActionResult> Post(Categoria categoria)
        {
            categoria.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(categoria);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return Ok(categoria);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una categoría con ese nombre."); //409
            }
        }

        // PUT /api/categorias
        [HttpPut]
        public async Task<ActionResult> Put(Categoria categoria)
        {
            var existe = await _context.Categorias.AnyAsync(x => x.Id == categoria.Id);
            if (!existe)
            {
                return NotFound("No existe una categoría con ese Id."); //404
            }

            var error = await ValidarAsync(categoria);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                return Ok(categoria);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una categoría con ese nombre."); //409
            }
        }

        // DELETE /api/categorias/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
            if (categoria == null)
            {
                return NotFound("No existe una categoría con ese Id."); //404
            }

            try
            {
                _context.Remove(categoria);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la categoría porque tiene habilidades o publicaciones asociadas."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Categoria categoria)
        {
            return Task.FromResult<string>(null);
        }
    }
}
