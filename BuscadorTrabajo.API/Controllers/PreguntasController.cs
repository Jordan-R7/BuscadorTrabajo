using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    // Preguntas que la empresa o el cliente agrega a su publicación (RN50, RN51).
    // Consultarlas es público (quien va a aplicar las necesita ver);
    // crearlas, editarlas o borrarlas solo lo puede hacer el dueño de la publicación o un administrador.
    [ApiController]
    [Route("/api/preguntas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Empresa,Cliente")]
    public class PreguntasController : ControllerBase
    {
        private readonly DataContext _context;

        public PreguntasController(DataContext context)
        {
            _context = context;
        }

        // GET /api/preguntas?publicacion=5
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Get([FromQuery] int? publicacion)
        {
            var consulta = _context.Preguntas.AsNoTracking();
            if (publicacion != null)
            {
                consulta = consulta.Where(x => x.IdPublicacion == publicacion);
            }

            return Ok(await consulta
                .OrderBy(x => x.IdPublicacion)
                .ThenBy(x => x.Orden)
                .ThenBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/preguntas/5
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> Get(int id)
        {
            var pregunta = await _context.Preguntas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound("No existe una pregunta con ese Id."); //404
            }

            return Ok(pregunta);
        }

        // POST /api/preguntas
        [HttpPost]
        public async Task<ActionResult> Post(Pregunta pregunta)
        {
            pregunta.Id = 0;
            Limpiar(pregunta);

            var error = await ValidarPublicacionAsync(pregunta.IdPublicacion);
            if (error != null)
            {
                return error;
            }

            _context.Add(pregunta);
            await _context.SaveChangesAsync();
            return Ok(pregunta);
        }

        // PUT /api/preguntas
        [HttpPut]
        public async Task<ActionResult> Put(Pregunta pregunta)
        {
            var actual = await _context.Preguntas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pregunta.Id);
            if (actual == null)
            {
                return NotFound("No existe una pregunta con ese Id."); //404
            }

            if (actual.IdPublicacion != pregunta.IdPublicacion)
            {
                return BadRequest("No se puede cambiar la publicación de una pregunta."); //400
            }

            Limpiar(pregunta);
            var error = await ValidarPublicacionAsync(pregunta.IdPublicacion);
            if (error != null)
            {
                return error;
            }

            _context.Update(pregunta);
            await _context.SaveChangesAsync();
            return Ok(pregunta);
        }

        // DELETE /api/preguntas/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pregunta = await _context.Preguntas.FirstOrDefaultAsync(x => x.Id == id);
            if (pregunta == null)
            {
                return NotFound("No existe una pregunta con ese Id."); //404
            }

            var error = await ValidarPublicacionAsync(pregunta.IdPublicacion);
            if (error != null)
            {
                return error;
            }

            try
            {
                _context.Remove(pregunta);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("La pregunta ya tiene respuestas y no se puede borrar."); //409
            }
        }

        // Revisa que la publicación exista (RN50), que el usuario sea su dueño (RN51)
        // y que todavía no tenga aplicaciones: si ya hay aplicaciones, cambiar las preguntas
        // dejaría aplicaciones sin responder preguntas obligatorias (RN55).
        private async Task<ActionResult> ValidarPublicacionAsync(int idPublicacion)
        {
            var publicacion = await _context.Publicaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idPublicacion);
            if (publicacion == null)
            {
                return BadRequest("La publicación indicada no existe."); //400
            }

            if (!User.EsDuenoPublicacion(publicacion))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Solo el dueño de la publicación o un administrador puede gestionar sus preguntas."); //403
            }

            if (await _context.Aplicaciones.AnyAsync(x => x.IdPublicacion == idPublicacion))
            {
                return Conflict("La publicación ya recibió aplicaciones: sus preguntas ya no se pueden cambiar."); //409
            }

            return null;
        }

        private static void Limpiar(Pregunta pregunta)
        {
            pregunta.Texto = pregunta.Texto?.Trim();
            pregunta.Opciones = pregunta.TipoRespuesta == "OPCION_UNICA"
                ? string.Join(";", pregunta.ListaOpciones())
                : null;
        }
    }
}
