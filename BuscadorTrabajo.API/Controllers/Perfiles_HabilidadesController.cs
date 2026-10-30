using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/perfiles_habilidades")]
    public class Perfiles_HabilidadesController : ControllerBase
    {
        private readonly DataContext _context;

        public Perfiles_HabilidadesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/perfiles_habilidades
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Perfiles_Habilidades
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/perfiles_habilidades/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Perfiles_Habilidades
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

        // GET /api/perfiles_habilidades/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var perfilHabilidad = await _context.Perfiles_Habilidades
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (perfilHabilidad == null)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            return Ok(perfilHabilidad);
        }

        // POST /api/perfiles_habilidades
        [HttpPost]
        public async Task<ActionResult> Post(Perfil_Habilidad perfilHabilidad)
        {
            perfilHabilidad.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(perfilHabilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(perfilHabilidad);
                await _context.SaveChangesAsync();
                return Ok(perfilHabilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ese perfil ya tiene asignada esa habilidad."); //409
            }
        }

        // PUT /api/perfiles_habilidades
        [HttpPut]
        public async Task<ActionResult> Put(Perfil_Habilidad perfilHabilidad)
        {
            var existe = await _context.Perfiles_Habilidades.AnyAsync(x => x.Id == perfilHabilidad.Id);
            if (!existe)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            var error = await ValidarAsync(perfilHabilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(perfilHabilidad);
                await _context.SaveChangesAsync();
                return Ok(perfilHabilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ese perfil ya tiene asignada esa habilidad."); //409
            }
        }

        // DELETE /api/perfiles_habilidades/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perfilHabilidad = await _context.Perfiles_Habilidades.FirstOrDefaultAsync(x => x.Id == id);
            if (perfilHabilidad == null)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            try
            {
                _context.Remove(perfilHabilidad);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la asignación."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Perfil_Habilidad perfilHabilidad)
        {
            if (!await _context.PerfilesProfesionales.AnyAsync(x => x.Id == perfilHabilidad.IdPerfilProfesional))
            {
                return "El perfil profesional indicado no existe."; // RN32
            }

            if (!await _context.Habilidades.AnyAsync(x => x.Id == perfilHabilidad.IdHabilidad))
            {
                return "La habilidad indicada no existe."; // RN32
            }

            return null;
        }
    }
}
