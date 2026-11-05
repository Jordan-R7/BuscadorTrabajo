using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    // Las respuestas se crean junto con la aplicación (POST /api/aplicaciones).
    // Aquí solo se consultan.
    [ApiController]
    [Route("/api/respuestas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RespuestasController : ControllerBase
    {
        private readonly DataContext _context;

        public RespuestasController(DataContext context)
        {
            _context = context;
        }

        // GET /api/respuestas?aplicacion=5  ->  respuestas con el texto de su pregunta
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int aplicacion)
        {
            // Solo quien puede ver la aplicación puede ver sus respuestas
            if (!await _context.Aplicaciones.VisiblesPara(User).AnyAsync(x => x.Id == aplicacion))
            {
                return NotFound("No existe una aplicación con ese Id."); //404
            }

            return Ok(await _context.Respuestas
                .AsNoTracking()
                .Where(x => x.IdAplicacion == aplicacion)
                .OrderBy(x => x.Pregunta.Orden)
                .Select(x => new
                {
                    x.Id,
                    x.IdAplicacion,
                    x.IdPregunta,
                    Pregunta = x.Pregunta.Texto,
                    x.Pregunta.TipoRespuesta,
                    x.Valor,
                    x.FechaRespuesta
                })
                .ToListAsync());
        }
    }
}
