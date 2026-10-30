using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/habilidades")]
    public class HabilidadesController : ControllerBase
    {
        private readonly DataContext _context;

        public HabilidadesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/habilidades
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Habilidades
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/habilidades/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Habilidades
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

        // GET /api/habilidades/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var habilidad = await _context.Habilidades
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (habilidad == null)
            {
                return NotFound("No existe una habilidad con ese Id."); //404
            }

            return Ok(habilidad);
        }

        // POST /api/habilidades
        [HttpPost]
        public async Task<ActionResult> Post(Habilidad habilidad)
        {
            habilidad.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(habilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(habilidad);
                await _context.SaveChangesAsync();
                return Ok(habilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una habilidad con ese nombre en esa categoría."); //409
            }
        }

        // PUT /api/habilidades
        [HttpPut]
        public async Task<ActionResult> Put(Habilidad habilidad)
        {
            var existe = await _context.Habilidades.AnyAsync(x => x.Id == habilidad.Id);
            if (!existe)
            {
                return NotFound("No existe una habilidad con ese Id."); //404
            }

            var error = await ValidarAsync(habilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(habilidad);
                await _context.SaveChangesAsync();
                return Ok(habilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una habilidad con ese nombre en esa categoría."); //409
            }
        }

        // DELETE /api/habilidades/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var habilidad = await _context.Habilidades.FirstOrDefaultAsync(x => x.Id == id);
            if (habilidad == null)
            {
                return NotFound("No existe una habilidad con ese Id."); //404
            }

            try
            {
                _context.Remove(habilidad);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la habilidad porque está asignada a perfiles o publicaciones."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Habilidad habilidad)
        {
            if (!await _context.Categorias.AnyAsync(x => x.Id == habilidad.IdCategoria))
            {
                return "La categoría indicada no existe."; // RN14
            }

            return null;
        }
    }
}
