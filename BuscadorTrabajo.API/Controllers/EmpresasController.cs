using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/empresas")]
    public class EmpresasController : ControllerBase
    {
        private readonly DataContext _context;

        public EmpresasController(DataContext context)
        {
            _context = context;
        }

        // GET /api/empresas
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Empresas
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/empresas/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Empresas
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

        // GET /api/empresas/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (empresa == null)
            {
                return NotFound("No existe una empresa con ese Id."); //404
            }

            return Ok(empresa);
        }

        // POST /api/empresas
        [HttpPost]
        public async Task<ActionResult> Post(Empresa empresa)
        {
            empresa.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(empresa);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();
                return Ok(empresa);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una empresa con ese NIT."); //409
            }
        }

        // PUT /api/empresas
        [HttpPut]
        public async Task<ActionResult> Put(Empresa empresa)
        {
            var existe = await _context.Empresas.AnyAsync(x => x.Id == empresa.Id);
            if (!existe)
            {
                return NotFound("No existe una empresa con ese Id."); //404
            }

            var error = await ValidarAsync(empresa);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(empresa);
                await _context.SaveChangesAsync();
                return Ok(empresa);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe una empresa con ese NIT."); //409
            }
        }

        // DELETE /api/empresas/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var empresa = await _context.Empresas.FirstOrDefaultAsync(x => x.Id == id);
            if (empresa == null)
            {
                return NotFound("No existe una empresa con ese Id."); //404
            }

            try
            {
                _context.Remove(empresa);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la empresa porque tiene publicaciones asociadas."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Empresa empresa)
        {
            return Task.FromResult<string>(null);
        }
    }
}
