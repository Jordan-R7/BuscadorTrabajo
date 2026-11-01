using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/aplicaciones")]
    public class AplicacionesController : ControllerBase
    {
        private readonly DataContext _context;

        // Estados a los que puede pasar una aplicación desde cada estado (RN31)
        private static readonly Dictionary<string, string[]> TransicionesPermitidas = new()
        {
            ["PENDIENTE"] = new[] { "EN_REVISION", "RECHAZADA", "RETIRADA" },
            ["EN_REVISION"] = new[] { "ACEPTADA", "RECHAZADA", "RETIRADA" },
            ["ACEPTADA"] = Array.Empty<string>(),
            ["RECHAZADA"] = Array.Empty<string>(),
            ["RETIRADA"] = Array.Empty<string>()
        };

        public AplicacionesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/aplicaciones
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Aplicaciones
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/aplicaciones/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Aplicaciones
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

        // GET /api/aplicaciones/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var aplicacion = await _context.Aplicaciones
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (aplicacion == null)
            {
                return NotFound("No existe una aplicación con ese Id."); //404
            }

            return Ok(aplicacion);
        }

        // POST /api/aplicaciones
        [HttpPost]
        public async Task<ActionResult> Post(Aplicacion aplicacion)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            aplicacion.Id = 0;              // El Id lo genera la base de datos
            aplicacion.Estado = "PENDIENTE"; // Toda aplicación empieza pendiente (RN31)
            if (aplicacion.FechaAplicacion == default)
            {
                aplicacion.FechaAplicacion = hoy;
            }

            // La publicación y el desarrollador deben existir (RN27)
            var publicacion = await _context.Publicaciones
                .AsNoTracking()
                .Include(x => x.Cliente)
                .FirstOrDefaultAsync(x => x.Id == aplicacion.IdPublicacion);
            if (publicacion == null)
            {
                return BadRequest("La publicación indicada no existe."); //400
            }

            var desarrollador = await _context.Desarrolladores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == aplicacion.IdDesarrollador);
            if (desarrollador == null)
            {
                return BadRequest("El desarrollador indicado no existe."); //400
            }

            // Un desarrollador desactivado no puede aplicar a nuevas publicaciones
            if (!desarrollador.Activo)
            {
                return BadRequest("El desarrollador está inactivo y no puede aplicar."); //400
            }

            // El desarrollador debe tener perfil profesional (RN27)
            if (!await _context.PerfilesProfesionales.AnyAsync(x => x.IdDesarrollador == aplicacion.IdDesarrollador))
            {
                return BadRequest("El desarrollador debe tener un perfil profesional para poder aplicar."); //400
            }

            // Solo se aplica a publicaciones publicadas y vigentes (RN26)
            if (publicacion.Estado != "PUBLICADA"
                || (publicacion.FechaVencimiento != null && publicacion.FechaVencimiento < hoy))
            {
                return BadRequest("Solo se puede aplicar a publicaciones en estado PUBLICADA y que no estén vencidas."); //400
            }

            // No puede aplicar a una publicación de un cliente con su misma cédula (RN29)
            if (publicacion.Cliente != null && publicacion.Cliente.Cedula == desarrollador.Cedula)
            {
                return BadRequest("Un desarrollador no puede aplicar a su propia publicación."); //400
            }

            // No puede aplicar dos veces a la misma publicación (RN28)
            if (await _context.Aplicaciones.AnyAsync(x => x.IdPublicacion == aplicacion.IdPublicacion
                                                       && x.IdDesarrollador == aplicacion.IdDesarrollador))
            {
                return Conflict("El desarrollador ya aplicó a esta publicación."); //409
            }

            // Monto y plazo según el tipo de publicación (RN30)
            var error = ValidarPropuesta(aplicacion, publicacion.Tipo);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(aplicacion);
                await _context.SaveChangesAsync();
                return Ok(aplicacion);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("El desarrollador ya aplicó a esta publicación."); //409
            }
        }

        // PUT /api/aplicaciones
        [HttpPut]
        public async Task<ActionResult> Put(Aplicacion aplicacion)
        {
            var actual = await _context.Aplicaciones
                .AsNoTracking()
                .Include(x => x.Publicacion)
                .FirstOrDefaultAsync(x => x.Id == aplicacion.Id);

            if (actual == null)
            {
                return NotFound("No existe una aplicación con ese Id."); //404
            }

            // La publicación y el desarrollador de una aplicación no se cambian
            if (actual.IdPublicacion != aplicacion.IdPublicacion || actual.IdDesarrollador != aplicacion.IdDesarrollador)
            {
                return BadRequest("No se puede cambiar la publicación ni el desarrollador de una aplicación."); //400
            }

            // El estado solo cambia siguiendo las transiciones permitidas (RN31)
            if (actual.Estado != aplicacion.Estado && !TransicionValida(actual.Estado, aplicacion.Estado))
            {
                return BadRequest($"Una aplicación no puede pasar de {actual.Estado} a {aplicacion.Estado}."); //400
            }

            var error = ValidarPropuesta(aplicacion, actual.Publicacion.Tipo);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            _context.Update(aplicacion);
            await _context.SaveChangesAsync();
            return Ok(aplicacion);
        }

        // DELETE /api/aplicaciones/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var aplicacion = await _context.Aplicaciones.FirstOrDefaultAsync(x => x.Id == id);
            if (aplicacion == null)
            {
                return NotFound("No existe una aplicación con ese Id."); //404
            }

            _context.Remove(aplicacion);
            await _context.SaveChangesAsync();
            return NoContent(); //204
        }

        private static bool TransicionValida(string estadoActual, string estadoNuevo)
        {
            return estadoActual != null
                && TransicionesPermitidas.TryGetValue(estadoActual, out var permitidos)
                && permitidos.Contains(estadoNuevo);
        }

        // Servicios: monto y plazo obligatorios. Empleos: deben ir vacíos (RN30)
        private static string ValidarPropuesta(Aplicacion aplicacion, string tipoPublicacion)
        {
            if (tipoPublicacion == "SERVICIO")
            {
                if (aplicacion.MontoPropuesto == null || aplicacion.MontoPropuesto <= 0)
                {
                    return "Para aplicar a un servicio debe indicar un monto propuesto mayor que cero.";
                }

                if (aplicacion.PlazoEstimado == null)
                {
                    return "Para aplicar a un servicio debe indicar el plazo estimado en días.";
                }
            }
            else if (aplicacion.MontoPropuesto != null || aplicacion.PlazoEstimado != null)
            {
                return "Las aplicaciones a ofertas de empleo no llevan monto ni plazo propuestos.";
            }

            return null;
        }
    }
}
