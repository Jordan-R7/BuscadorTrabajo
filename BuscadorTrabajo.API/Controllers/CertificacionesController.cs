using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/certificaciones")]
    public class CertificacionesController : ControllerBase
    {
        private readonly DataContext _context;

        public CertificacionesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/certificaciones
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Certificaciones
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/certificaciones/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Certificaciones
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

        // GET /api/certificaciones/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var certificacion = await _context.Certificaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (certificacion == null)
            {
                return NotFound("No existe una certificación con ese Id."); //404
            }

            return Ok(certificacion);
        }

        // POST /api/certificaciones
        [HttpPost]
        public async Task<ActionResult> Post(Certificacion certificacion)
        {
            certificacion.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(certificacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(certificacion);
                await _context.SaveChangesAsync();
                return Ok(certificacion);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("La certificación ya está registrada."); //409
            }
        }

        // PUT /api/certificaciones
        [HttpPut]
        public async Task<ActionResult> Put(Certificacion certificacion)
        {
            var existe = await _context.Certificaciones.AnyAsync(x => x.Id == certificacion.Id);
            if (!existe)
            {
                return NotFound("No existe una certificación con ese Id."); //404
            }

            var error = await ValidarAsync(certificacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(certificacion);
                await _context.SaveChangesAsync();
                return Ok(certificacion);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("La certificación ya está registrada."); //409
            }
        }

        // DELETE /api/certificaciones/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var certificacion = await _context.Certificaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (certificacion == null)
            {
                return NotFound("No existe una certificación con ese Id."); //404
            }

            try
            {
                _context.Remove(certificacion);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la certificación."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Certificacion certificacion)
        {
            if (!await _context.PerfilesProfesionales.AnyAsync(x => x.Id == certificacion.IdPerfilProfesional))
            {
                return "El perfil profesional indicado no existe."; // RN09
            }

            return null;
        }
    }
}
