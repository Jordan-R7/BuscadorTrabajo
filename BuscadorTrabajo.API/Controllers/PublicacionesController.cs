using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/publicaciones")]
    public class PublicacionesController : ControllerBase
    {
        private readonly DataContext _context;

        // Estados a los que puede pasar una publicación desde cada estado (RN24)
        private static readonly Dictionary<string, string[]> TransicionesPermitidas = new()
        {
            ["BORRADOR"] = new[] { "PUBLICADA", "CANCELADA" },
            ["PUBLICADA"] = new[] { "PAUSADA", "CERRADA", "CANCELADA" },
            ["PAUSADA"] = new[] { "PUBLICADA", "CERRADA", "CANCELADA" },
            ["CERRADA"] = Array.Empty<string>(),
            ["CANCELADA"] = Array.Empty<string>()
        };

        public PublicacionesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/publicaciones
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Publicaciones
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/publicaciones/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Publicaciones
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

        // GET /api/publicaciones/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var publicacion = await _context.Publicaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (publicacion == null)
            {
                return NotFound("No existe una publicación con ese Id."); //404
            }

            return Ok(publicacion);
        }

        // POST /api/publicaciones
        [HttpPost]
        public async Task<ActionResult> Post(Publicacion publicacion)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            publicacion.Id = 0; // El Id lo genera la base de datos

            if (publicacion.FechaCreacion == default)
            {
                publicacion.FechaCreacion = hoy;
            }

            // Una publicación nueva empieza como borrador o directamente publicada (RN24)
            if (publicacion.Estado != "BORRADOR" && publicacion.Estado != "PUBLICADA")
            {
                return BadRequest("Una publicación nueva debe crearse en estado BORRADOR o PUBLICADA."); //400
            }

            if (publicacion.Estado == "PUBLICADA" && publicacion.FechaPublicacion == null)
            {
                publicacion.FechaPublicacion = hoy;
            }

            var error = await ValidarRelacionesAsync(publicacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            _context.Add(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);
        }

        // PUT /api/publicaciones
        [HttpPut]
        public async Task<ActionResult> Put(Publicacion publicacion)
        {
            var actual = await _context.Publicaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == publicacion.Id);

            if (actual == null)
            {
                return NotFound("No existe una publicación con ese Id."); //404
            }

            // El estado solo cambia siguiendo las transiciones permitidas (RN24)
            if (actual.Estado != publicacion.Estado && !TransicionValida(actual.Estado, publicacion.Estado))
            {
                return BadRequest($"Una publicación no puede pasar de {actual.Estado} a {publicacion.Estado}."); //400
            }

            if (publicacion.Estado == "PUBLICADA" && publicacion.FechaPublicacion == null)
            {
                publicacion.FechaPublicacion = DateOnly.FromDateTime(DateTime.Today);
            }

            var error = await ValidarRelacionesAsync(publicacion);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            _context.Update(publicacion);
            await _context.SaveChangesAsync();
            return Ok(publicacion);
        }

        // DELETE /api/publicaciones/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var publicacion = await _context.Publicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacion == null)
            {
                return NotFound("No existe una publicación con ese Id."); //404
            }

            // Una publicación con aplicaciones no se borra: se cierra o se cancela (RN25)
            if (await _context.Aplicaciones.AnyAsync(x => x.IdPublicacion == id))
            {
                return Conflict("La publicación tiene aplicaciones. En lugar de eliminarla, ciérrela o cancélela."); //409
            }

            try
            {
                _context.Remove(publicacion);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la publicación porque tiene registros asociados."); //409
            }
        }

        private static bool TransicionValida(string estadoActual, string estadoNuevo)
        {
            return estadoActual != null
                && TransicionesPermitidas.TryGetValue(estadoActual, out var permitidos)
                && permitidos.Contains(estadoNuevo);
        }

        // Comprueba que existan la empresa, el cliente y la categoría indicados (RN17, RN19)
        private async Task<string> ValidarRelacionesAsync(Publicacion publicacion)
        {
            if (publicacion.IdEmpresa != null && !await _context.Empresas.AnyAsync(x => x.Id == publicacion.IdEmpresa))
            {
                return "La empresa indicada no existe.";
            }

            if (publicacion.IdCliente != null && !await _context.Clientes.AnyAsync(x => x.Id == publicacion.IdCliente))
            {
                return "El cliente indicado no existe.";
            }

            if (!await _context.Categorias.AnyAsync(x => x.Id == publicacion.IdCategoria))
            {
                return "La categoría indicada no existe.";
            }

            return null;
        }
    }
}
