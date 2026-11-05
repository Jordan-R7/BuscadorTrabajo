using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/publicaciones_habilidades")]
    // Consultar (GET) es público; crear, editar y borrar requiere iniciar sesión con rol Admin, Empresa, Cliente
    public class Publicaciones_HabilidadesController : ControllerBase
    {
        private readonly DataContext _context;

        public Publicaciones_HabilidadesController(DataContext context)
        {
            _context = context;
        }

        // GET /api/publicaciones_habilidades
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Publicaciones_Habilidades
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/publicaciones_habilidades/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Publicaciones_Habilidades
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

        // GET /api/publicaciones_habilidades/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var publicacionHabilidad = await _context.Publicaciones_Habilidades
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (publicacionHabilidad == null)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            return Ok(publicacionHabilidad);
        }

        // POST /api/publicaciones_habilidades
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Empresa,Cliente")]
        [HttpPost]
        public async Task<ActionResult> Post(Publicacion_Habilidad publicacionHabilidad)
        {
            publicacionHabilidad.Id = 0; // El Id lo genera la base de datos

            if (!await PuedeGestionarAsync(publicacionHabilidad.IdPublicacion))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Solo el dueño de la publicación puede cambiar sus habilidades."); //403
            }

            var error = await ValidarAsync(publicacionHabilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(publicacionHabilidad);
                await _context.SaveChangesAsync();
                return Ok(publicacionHabilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Esa publicación ya tiene asignada esa habilidad."); //409
            }
        }

        // PUT /api/publicaciones_habilidades
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Empresa,Cliente")]
        [HttpPut]
        public async Task<ActionResult> Put(Publicacion_Habilidad publicacionHabilidad)
        {
            var actual = await _context.Publicaciones_Habilidades.AsNoTracking().FirstOrDefaultAsync(x => x.Id == publicacionHabilidad.Id);
            if (actual == null)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            if (!await PuedeGestionarAsync(actual.IdPublicacion) || !await PuedeGestionarAsync(publicacionHabilidad.IdPublicacion))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Solo el dueño de la publicación puede cambiar sus habilidades."); //403
            }

            var error = await ValidarAsync(publicacionHabilidad);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(publicacionHabilidad);
                await _context.SaveChangesAsync();
                return Ok(publicacionHabilidad);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Esa publicación ya tiene asignada esa habilidad."); //409
            }
        }

        // DELETE /api/publicaciones_habilidades/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Empresa,Cliente")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var publicacionHabilidad = await _context.Publicaciones_Habilidades.FirstOrDefaultAsync(x => x.Id == id);
            if (publicacionHabilidad == null)
            {
                return NotFound("No existe esa asignación de habilidad."); //404
            }

            if (!await PuedeGestionarAsync(publicacionHabilidad.IdPublicacion))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Solo el dueño de la publicación puede cambiar sus habilidades."); //403
            }

            try
            {
                _context.Remove(publicacionHabilidad);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar la asignación."); //409
            }
        }

        // true si el usuario es dueño de la publicación o administrador.
        // Si la publicación no existe se deja pasar: ValidarAsync responde el error 400 correspondiente.
        private async Task<bool> PuedeGestionarAsync(int idPublicacion)
        {
            var publicacion = await _context.Publicaciones.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idPublicacion);
            return publicacion == null || User.EsDuenoPublicacion(publicacion);
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private async Task<string> ValidarAsync(Publicacion_Habilidad publicacionHabilidad)
        {
            if (!await _context.Publicaciones.AnyAsync(x => x.Id == publicacionHabilidad.IdPublicacion))
            {
                return "La publicación indicada no existe."; // RN32
            }

            if (!await _context.Habilidades.AnyAsync(x => x.Id == publicacionHabilidad.IdHabilidad))
            {
                return "La habilidad indicada no existe."; // RN32
            }

            return null;
        }
    }
}
