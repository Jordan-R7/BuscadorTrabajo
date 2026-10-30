using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/administradores")]
    public class AdministradoresController : ControllerBase
    {
        private readonly DataContext _context;

        public AdministradoresController(DataContext context)
        {
            _context = context;
        }

        // GET /api/administradores
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Administradores
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/administradores/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Administradores
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

        // GET /api/administradores/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var administrador = await _context.Administradores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (administrador == null)
            {
                return NotFound("No existe un administrador con ese Id."); //404
            }

            return Ok(administrador);
        }

        // POST /api/administradores
        [HttpPost]
        public async Task<ActionResult> Post(Administrador administrador)
        {
            administrador.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(administrador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(administrador);
                await _context.SaveChangesAsync();
                return Ok(administrador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un administrador con esa cédula."); //409
            }
        }

        // PUT /api/administradores
        [HttpPut]
        public async Task<ActionResult> Put(Administrador administrador)
        {
            var existe = await _context.Administradores.AnyAsync(x => x.Id == administrador.Id);
            if (!existe)
            {
                return NotFound("No existe un administrador con ese Id."); //404
            }

            var error = await ValidarAsync(administrador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(administrador);
                await _context.SaveChangesAsync();
                return Ok(administrador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un administrador con esa cédula."); //409
            }
        }

        // DELETE /api/administradores/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var administrador = await _context.Administradores.FirstOrDefaultAsync(x => x.Id == id);
            if (administrador == null)
            {
                return NotFound("No existe un administrador con ese Id."); //404
            }

            try
            {
                _context.Remove(administrador);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el administrador."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Administrador administrador)
        {
            return Task.FromResult<string>(null);
        }
    }
}
