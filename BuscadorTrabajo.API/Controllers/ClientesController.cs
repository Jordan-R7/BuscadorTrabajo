using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly DataContext _context;

        public ClientesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/clientes
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Clientes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/clientes/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Clientes
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

        // GET /api/clientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (cliente == null)
            {
                return NotFound("No existe un cliente con ese Id."); //404
            }

            return Ok(cliente);
        }

        // POST /api/clientes
        [HttpPost]
        public async Task<ActionResult> Post(Cliente cliente)
        {
            cliente.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(cliente);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return Ok(cliente);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un cliente con esa cédula."); //409
            }
        }

        // PUT /api/clientes
        [HttpPut]
        public async Task<ActionResult> Put(Cliente cliente)
        {
            var existe = await _context.Clientes.AnyAsync(x => x.Id == cliente.Id);
            if (!existe)
            {
                return NotFound("No existe un cliente con ese Id."); //404
            }

            var error = await ValidarAsync(cliente);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                return Ok(cliente);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un cliente con esa cédula."); //409
            }
        }

        // DELETE /api/clientes/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(x => x.Id == id);
            if (cliente == null)
            {
                return NotFound("No existe un cliente con ese Id."); //404
            }

            try
            {
                _context.Remove(cliente);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el cliente porque tiene publicaciones asociadas."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Cliente cliente)
        {
            return Task.FromResult<string>(null);
        }
    }
}
