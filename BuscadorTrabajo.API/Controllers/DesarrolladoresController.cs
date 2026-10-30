using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/desarrolladores")]
    public class DesarrolladoresController : ControllerBase
    {
        private readonly DataContext _context;

        public DesarrolladoresController(DataContext context)
        {
            _context = context;
        }

        // GET /api/desarrolladores
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Desarrolladores
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/desarrolladores/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Desarrolladores
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

        // GET /api/desarrolladores/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var desarrollador = await _context.Desarrolladores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            return Ok(desarrollador);
        }

        // POST /api/desarrolladores
        [HttpPost]
        public async Task<ActionResult> Post(Desarrollador desarrollador)
        {
            desarrollador.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(desarrollador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(desarrollador);
                await _context.SaveChangesAsync();
                return Ok(desarrollador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un desarrollador con esa cédula."); //409
            }
        }

        // PUT /api/desarrolladores
        [HttpPut]
        public async Task<ActionResult> Put(Desarrollador desarrollador)
        {
            var existe = await _context.Desarrolladores.AnyAsync(x => x.Id == desarrollador.Id);
            if (!existe)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            var error = await ValidarAsync(desarrollador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(desarrollador);
                await _context.SaveChangesAsync();
                return Ok(desarrollador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un desarrollador con esa cédula."); //409
            }
        }

        // DELETE /api/desarrolladores/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var desarrollador = await _context.Desarrolladores.FirstOrDefaultAsync(x => x.Id == id);
            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            try
            {
                _context.Remove(desarrollador);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el desarrollador porque tiene un perfil profesional o aplicaciones asociadas."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Desarrollador desarrollador)
        {
            return Task.FromResult<string>(null);
        }
    }
}
