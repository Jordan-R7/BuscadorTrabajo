using BuscadorTrabajo.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    // Entrega las imágenes guardadas para mostrarlas con <img src="...">
    [ApiController]
    [Route("/api/imagenes")]
    public class ImagenesController : ControllerBase
    {
        private readonly DataContext _context;

        public ImagenesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/imagenes/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var imagen = await _context.Imagenes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (imagen == null)
            {
                return NotFound("No existe una imagen con ese Id."); //404
            }

            // Cada foto nueva tiene un Id nuevo, así que el navegador la puede guardar en caché
            Response.Headers.CacheControl = "public, max-age=604800";
            return File(imagen.Datos, imagen.TipoContenido);
        }
    }
}
