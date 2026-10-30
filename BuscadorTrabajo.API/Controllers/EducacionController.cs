using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/educacion")]
    public class EducacionController : ControllerBase
    {
        private readonly DataContext _context;

        public EducacionController(DataContext context)
        {
            _context = context;
        }

        // GET /api/educacion
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Educaciones
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/educacion/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Educaciones
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

        // GET /api/educacion/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var educacion = await _context.Educaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (educacion == null)
            {
                return NotFound("No existe un registro de educación con ese Id."); //404
            }

            return Ok(educacion);
        }

        // POST /api/educacion
        [HttpPost]
        public async Task<ActionResult> Post(Educacion educacion)
        {
            educacion.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(educacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(educacion);
                await _context.SaveChangesAsync();
                return Ok(educacion);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("El registro de educación ya existe."); //409
            }
        }

        // PUT /api/educacion
        [HttpPut]
        public async Task<ActionResult> Put(Educacion educacion)
        {
            var existe = await _context.Educaciones.AnyAsync(x => x.Id == educacion.Id);
            if (!existe)
            {
                return NotFound("No existe un registro de educación con ese Id."); //404
            }

            var error = await ValidarAsync(educacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(educacion);
                await _context.SaveChangesAsync();
                return Ok(educacion);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("El registro de educación ya existe."); //409
            }
        }

        // DELETE /api/educacion/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var educacion = await _context.Educaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (educacion == null)
            {
                return NotFound("No existe un registro de educación con ese Id."); //404
            }

            try
            {
                _context.Remove(educacion);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el registro de educación."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Educacion educacion)
        {
            if (!await _context.PerfilesProfesionales.AnyAsync(x => x.Id == educacion.IdPerfilProfesional))
            {
                return "El perfil profesional indicado no existe."; // RN09
            }

            return null;
        }
    }
}
