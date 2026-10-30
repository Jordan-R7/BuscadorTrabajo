using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/experiencias")]
    public class ExperienciasController : ControllerBase
    {
        private readonly DataContext _context;

        public ExperienciasController(DataContext context)
        {
            _context = context;
        }

        // GET /api/experiencias
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Experiencias
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/experiencias/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Experiencias
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

        // GET /api/experiencias/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var experiencia = await _context.Experiencias
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (experiencia == null)
            {
                return NotFound("No existe una experiencia con ese Id."); //404
            }

            return Ok(experiencia);
        }

        // POST /api/experiencias
        [HttpPost]
        public async Task<ActionResult> Post(Experiencia experiencia)
        {
            experiencia.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(experiencia);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(experiencia);
                await _context.SaveChangesAsync();
                return Ok(experiencia);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("La experiencia ya está registrada."); //409
            }
        }

        // PUT /api/experiencias
        [HttpPut]
        public async Task<ActionResult> Put(Experiencia experiencia)
        {
            var existe = await _context.Experiencias.AnyAsync(x => x.Id == experiencia.Id);
            if (!existe)
            {
                return NotFound("No existe una experiencia con ese Id."); //404
            }

            var error = await ValidarAsync(experiencia);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(experiencia);
                await _context.SaveChangesAsync();
                return Ok(experiencia);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("La experiencia ya está registrada."); //409
            }
        }

        // DELETE /api/experiencias/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var experiencia = await _context.Experiencias.FirstOrDefaultAsync(x => x.Id == id);
            if (experiencia == null)
            {
                return NotFound("No existe una experiencia con ese Id."); //404
            }

            try
            {
                _context.Remove(experiencia);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la experiencia."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Experiencia experiencia)
        {
            if (!await _context.PerfilesProfesionales.AnyAsync(x => x.Id == experiencia.IdPerfilProfesional))
            {
                return "El perfil profesional indicado no existe."; // RN09
            }

            return null;
        }
    }
}
