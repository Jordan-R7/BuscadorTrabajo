using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/perfilesprofesionales")]
    public class PerfilesProfesionalesController : ControllerBase
    {
        private readonly DataContext _context;

        public PerfilesProfesionalesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/perfilesprofesionales
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.PerfilesProfesionales
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/perfilesprofesionales/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.PerfilesProfesionales
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

        // GET /api/perfilesprofesionales/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var perfil = await _context.PerfilesProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (perfil == null)
            {
                return NotFound("No existe un perfil profesional con ese Id."); //404
            }

            return Ok(perfil);
        }

        // POST /api/perfilesprofesionales
        [HttpPost]
        public async Task<ActionResult> Post(PerfilProfesional perfil)
        {
            perfil.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(perfil);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(perfil);
                await _context.SaveChangesAsync();
                return Ok(perfil);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Este desarrollador ya tiene un perfil profesional."); //409
            }
        }

        // PUT /api/perfilesprofesionales
        [HttpPut]
        public async Task<ActionResult> Put(PerfilProfesional perfil)
        {
            var existe = await _context.PerfilesProfesionales.AnyAsync(x => x.Id == perfil.Id);
            if (!existe)
            {
                return NotFound("No existe un perfil profesional con ese Id."); //404
            }

            var error = await ValidarAsync(perfil);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(perfil);
                await _context.SaveChangesAsync();
                return Ok(perfil);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Este desarrollador ya tiene un perfil profesional."); //409
            }
        }

        // DELETE /api/perfilesprofesionales/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perfil = await _context.PerfilesProfesionales.FirstOrDefaultAsync(x => x.Id == id);
            if (perfil == null)
            {
                return NotFound("No existe un perfil profesional con ese Id."); //404
            }

            try
            {
                _context.Remove(perfil);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el perfil profesional."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(PerfilProfesional perfil)
        {
            if (!await _context.Desarrolladores.AnyAsync(x => x.Id == perfil.IdDesarrollador))
            {
                return "El desarrollador indicado no existe."; // RN07
            }

            return null;
        }
    }
}
