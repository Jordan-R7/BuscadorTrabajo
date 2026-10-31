using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuscadorTrabajo.Shared.DTOs;

namespace BuscadorTrabajo.API.Controllers
{
    [ApiController]
    [Route("/api/desarrolladores")]
    public class DesarrolladoresController : ControllerBase
    {
        private readonly DataContext _context;

        public DesarrolladoresController(DataContext context)
        {
            _context = context;
        }

        // GET /api/desarrolladores
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _context.Desarrolladores
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync());
        }

        // GET /api/desarrolladores/paginated?page=1&pageSize=5
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginated(int page = 1, int pageSize = 5)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var query = _context.Desarrolladores
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

        // GET /api/desarrolladores/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var desarrollador = await _context.Desarrolladores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            return Ok(desarrollador);
        }

        // GET /api/desarrolladores/5/perfil-completo
        [HttpGet("{id:int}/perfil-completo")]
        public async Task<ActionResult> GetPerfilCompleto(int id)
        {
            // 1. Buscar los datos personales.
            var desarrollador = await _context.Desarrolladores
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id.");
            }

            // 2. Buscar el perfil profesional.
            var perfil = await _context.PerfilesProfesionales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdDesarrollador == id);

            if (perfil == null)
            {
                return NotFound("El desarrollador no tiene un perfil profesional registrado.");
            }

            // 3. Consultar la educación.
            var educaciones = await _context.Educaciones
                .AsNoTracking()
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .Select(x => new EducacionRegistroDTO
                {
                    Id = x.Id,
                    NivelEducativo = x.NivelEducativo,
                    Institucion = x.Institucion,
                    Titulo = x.Titulo,
                    FechaInicio = x.FechaInicio,
                    FechaFin = x.FechaFin,
                    EnCurso = x.EnCurso
                })
                .ToListAsync();

            // 4. Consultar la experiencia laboral.
            var experiencias = await _context.Experiencias
                .AsNoTracking()
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .Select(x => new ExperienciaRegistroDTO
                {
                    Id = x.Id,
                    Empresa = x.Empresa,
                    Cargo = x.Cargo,
                    Descripcion = x.Descripcion,
                    FechaInicio = x.FechaInicio,
                    FechaFin = x.FechaFin,
                    EstadoActual = x.EstadoActual
                })
                .ToListAsync();

            // 5. Consultar las certificaciones.
            var certificaciones = await _context.Certificaciones
                .AsNoTracking()
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .Select(x => new CertificacionRegistroDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Institucion = x.Institucion,
                    FechaEmision = x.FechaEmision,
                    FechaVencimiento = x.FechaVencimiento,
                    UrlCertificado = x.UrlCertificado
                })
                .ToListAsync();

            // 6. Consultar las habilidades asociadas.
            var habilidadesIds = await _context.Perfiles_Habilidades
                .AsNoTracking()
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .Select(x => x.IdHabilidad)
                .ToListAsync();

            // 7. Construir el DTO con toda la información.
            var resultado = new RegistroDesarrolladorDTO
            {
                DatosPersonales = new DesarrolladorRegistroDTO
                {
                    Cedula = desarrollador.Cedula,
                    Nombre = desarrollador.Nombre,
                    Apellido = desarrollador.Apellido,
                    Celular = desarrollador.Celular,
                    Correo = desarrollador.Correo,
                    FechaNacimiento = desarrollador.FechaNacimiento
                },

                PerfilProfesional = new PerfilProfesionalRegistroDTO
                {
                    TituloProfesional = perfil.TituloProfesional,
                    Descripcion = perfil.Descripcion,
                    Pais = perfil.Pais,
                    Ciudad = perfil.Ciudad,
                    Modalidad = perfil.Modalidad,
                    GitHub = perfil.GitHub,
                    LinkedIn = perfil.LinkedIn
                },

                Educaciones = educaciones,
                Experiencias = experiencias,
                Certificaciones = certificaciones,
                HabilidadesIds = habilidadesIds
            };

            return Ok(resultado);
        }

        // POST /api/desarrolladores
        [HttpPost]
        public async Task<ActionResult> Post(Desarrollador desarrollador)
        {
            desarrollador.Id = 0; // El Id lo genera la base de datos

            var error = await ValidarAsync(desarrollador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Add(desarrollador);
                await _context.SaveChangesAsync();
                return Ok(desarrollador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un desarrollador con esa cédula."); //409
            }
        }

        [HttpPost("registro-completo")]
        public async Task<ActionResult> RegistrarCompleto(
        [FromBody] RegistroDesarrolladorDTO registro)
        {
            // Validar la selección de habilidades.
            if (registro == null || registro.HabilidadesIds == null)
            {
                return BadRequest("La información de las habilidades no es válida.");
            }

            if (registro.HabilidadesIds.Any(x => x <= 0))
            {
                return BadRequest("Se recibió un identificador de habilidad inválido.");
            }

            if (registro.HabilidadesIds.Distinct().Count() != registro.HabilidadesIds.Count)
            {
                return BadRequest("No se pueden registrar habilidades repetidas.");
            }

            // Comprobar que todas las habilidades seleccionadas existan.
            var habilidadesValidas = await _context.Habilidades
                .Where(x => registro.HabilidadesIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (habilidadesValidas.Count != registro.HabilidadesIds.Count)
            {
                return BadRequest("Una o más habilidades seleccionadas no existen.");
            }
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear los datos personales del desarrollador.
                var desarrollador = new Desarrollador
                {
                    Cedula = registro.DatosPersonales.Cedula,
                    Nombre = registro.DatosPersonales.Nombre,
                    Apellido = registro.DatosPersonales.Apellido,
                    Celular = registro.DatosPersonales.Celular,
                    Correo = registro.DatosPersonales.Correo,
                    FechaNacimiento = registro.DatosPersonales.FechaNacimiento
                };

                _context.Desarrolladores.Add(desarrollador);
                await _context.SaveChangesAsync();

                // 2. Crear el perfil profesional asociado al desarrollador.
                var perfil = new PerfilProfesional
                {
                    IdDesarrollador = desarrollador.Id,
                    TituloProfesional = registro.PerfilProfesional.TituloProfesional,
                    Descripcion = registro.PerfilProfesional.Descripcion,
                    Pais = registro.PerfilProfesional.Pais,
                    Ciudad = registro.PerfilProfesional.Ciudad,
                    Modalidad = registro.PerfilProfesional.Modalidad,
                    GitHub = registro.PerfilProfesional.GitHub,
                    LinkedIn = registro.PerfilProfesional.LinkedIn
                };

                _context.PerfilesProfesionales.Add(perfil);
                await _context.SaveChangesAsync();

                // 3. Registrar la formación académica.
                var educaciones = registro.Educaciones.Select(e => new Educacion
                {
                    IdPerfilProfesional = perfil.Id,
                    NivelEducativo = e.NivelEducativo,
                    Institucion = e.Institucion,
                    Titulo = e.Titulo,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    EnCurso = e.EnCurso
                }).ToList();

                // 4. Registrar la experiencia laboral.
                var experiencias = registro.Experiencias.Select(e => new Experiencia
                {
                    IdPerfilProfesional = perfil.Id,
                    Empresa = e.Empresa,
                    Cargo = e.Cargo,
                    Descripcion = e.Descripcion,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    EstadoActual = e.EstadoActual
                }).ToList();

                // 5. Registrar las certificaciones.
                var certificaciones = registro.Certificaciones.Select(c => new Certificacion
                {
                    IdPerfilProfesional = perfil.Id,
                    Titulo = c.Titulo,
                    Institucion = c.Institucion,
                    FechaEmision = c.FechaEmision,
                    FechaVencimiento = c.FechaVencimiento,
                    UrlCertificado = c.UrlCertificado
                }).ToList();

                // 6. Agregar los registros relacionados.
                _context.Educaciones.AddRange(educaciones);
                _context.Experiencias.AddRange(experiencias);
                _context.Certificaciones.AddRange(certificaciones);

                var habilidadesPerfil = registro.HabilidadesIds
                .Select(idHabilidad => new Perfil_Habilidad
                {
                    IdPerfilProfesional = perfil.Id,
                    IdHabilidad = idHabilidad
                })
                .ToList();

                _context.Perfiles_Habilidades.AddRange(habilidadesPerfil);

                await _context.SaveChangesAsync();

                // 7. Confirmar toda la operación.
                await transaction.CommitAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = desarrollador.Id },
                    new
                    {
                        mensaje = "El registro del desarrollador se completó correctamente.",
                        idDesarrollador = desarrollador.Id,
                        idPerfilProfesional = perfil.Id
                    });
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                await transaction.RollbackAsync();

                return Conflict(
                    "Ya existe un desarrollador registrado con esa cédula.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT /api/desarrolladores
        [HttpPut]
        public async Task<ActionResult> Put(Desarrollador desarrollador)
        {
            var existe = await _context.Desarrolladores.AnyAsync(x => x.Id == desarrollador.Id);
            if (!existe)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            var error = await ValidarAsync(desarrollador);
            if (error != null)
            {
                return BadRequest(error); //400
            }

            try
            {
                _context.Update(desarrollador);
                await _context.SaveChangesAsync();
                return Ok(desarrollador);
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                return Conflict("Ya existe un desarrollador con esa cédula."); //409
            }
        }

        // PUT /api/desarrolladores/5/perfil-completo
        [HttpPut("{id:int}/perfil-completo")]
        public async Task<ActionResult> ActualizarPerfilCompleto(
            int id,
            [FromBody] RegistroDesarrolladorDTO registro)
        {
            // 1. Validar que la información principal esté completa.
            if (registro == null ||
                registro.DatosPersonales == null ||
                registro.PerfilProfesional == null ||
                registro.Educaciones == null ||
                registro.Experiencias == null ||
                registro.Certificaciones == null ||
                registro.HabilidadesIds == null)
            {
                return BadRequest("La información del perfil está incompleta.");
            }

            // 2. Obtener el desarrollador.
            var desarrollador = await _context.Desarrolladores
                .FirstOrDefaultAsync(x => x.Id == id);

            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id.");
            }

            // 3. Obtener su perfil profesional.
            var perfil = await _context.PerfilesProfesionales
                .FirstOrDefaultAsync(x => x.IdDesarrollador == id);

            if (perfil == null)
            {
                return NotFound("El desarrollador no tiene un perfil profesional.");
            }

            // 4. Verificar que la cédula no pertenezca a otro desarrollador.
            var cedulaExiste = await _context.Desarrolladores.AnyAsync(
                x => x.Cedula == registro.DatosPersonales.Cedula &&
                     x.Id != id);

            if (cedulaExiste)
            {
                return Conflict("La cédula ya está registrada por otro desarrollador.");
            }

            // 5. Consultar los registros actuales relacionados con el perfil.
            var educacionesActuales = await _context.Educaciones
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .ToListAsync();

            var experienciasActuales = await _context.Experiencias
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .ToListAsync();

            var certificacionesActuales = await _context.Certificaciones
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .ToListAsync();

            var habilidadesActuales = await _context.Perfiles_Habilidades
                .Where(x => x.IdPerfilProfesional == perfil.Id)
                .ToListAsync();

            // 6. Obtener los identificadores enviados.
            var idsEducacion = registro.Educaciones
                .Where(x => x.Id > 0)
                .Select(x => x.Id)
                .ToList();

            var idsExperiencia = registro.Experiencias
                .Where(x => x.Id > 0)
                .Select(x => x.Id)
                .ToList();

            var idsCertificaciones = registro.Certificaciones
                .Where(x => x.Id > 0)
                .Select(x => x.Id)
                .ToList();

            // 7. Evitar identificadores negativos o repetidos.
            if (registro.Educaciones.Any(x => x.Id < 0) ||
                registro.Experiencias.Any(x => x.Id < 0) ||
                registro.Certificaciones.Any(x => x.Id < 0))
            {
                return BadRequest("Se recibió un identificador de registro inválido.");
            }

            if (idsEducacion.Distinct().Count() != idsEducacion.Count ||
                idsExperiencia.Distinct().Count() != idsExperiencia.Count ||
                idsCertificaciones.Distinct().Count() != idsCertificaciones.Count)
            {
                return BadRequest("Se enviaron identificadores duplicados.");
            }

            // 8. Verificar que los registros pertenezcan al perfil solicitado.
            if (idsEducacion.Any(idRegistro =>
                !educacionesActuales.Any(x => x.Id == idRegistro)))
            {
                return BadRequest("Una educación no pertenece a este perfil profesional.");
            }

            if (idsExperiencia.Any(idRegistro =>
                !experienciasActuales.Any(x => x.Id == idRegistro)))
            {
                return BadRequest("Una experiencia no pertenece a este perfil profesional.");
            }

            if (idsCertificaciones.Any(idRegistro =>
                !certificacionesActuales.Any(x => x.Id == idRegistro)))
            {
                return BadRequest("Una certificación no pertenece a este perfil profesional.");
            }

            // 9. Validar las habilidades recibidas.
            if (registro.HabilidadesIds.Any(x => x <= 0) ||
                registro.HabilidadesIds.Distinct().Count() != registro.HabilidadesIds.Count)
            {
                return BadRequest("La selección de habilidades contiene identificadores inválidos o repetidos.");
            }

            var habilidadesSolicitadas = registro.HabilidadesIds;

            var habilidadesValidas = await _context.Habilidades
                .Where(x => habilidadesSolicitadas.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (habilidadesValidas.Count != habilidadesSolicitadas.Count)
            {
                return BadRequest("Una o más habilidades no existen.");
            }

            // 10. Comenzar la transacción.
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 11. Actualizar los datos personales.
                desarrollador.Cedula = registro.DatosPersonales.Cedula;
                desarrollador.Nombre = registro.DatosPersonales.Nombre;
                desarrollador.Apellido = registro.DatosPersonales.Apellido;
                desarrollador.Celular = registro.DatosPersonales.Celular;
                desarrollador.Correo = registro.DatosPersonales.Correo;
                desarrollador.FechaNacimiento = registro.DatosPersonales.FechaNacimiento;

                // 12. Actualizar el perfil profesional.
                perfil.TituloProfesional = registro.PerfilProfesional.TituloProfesional;
                perfil.Descripcion = registro.PerfilProfesional.Descripcion;
                perfil.Pais = registro.PerfilProfesional.Pais;
                perfil.Ciudad = registro.PerfilProfesional.Ciudad;
                perfil.Modalidad = registro.PerfilProfesional.Modalidad;
                perfil.GitHub = registro.PerfilProfesional.GitHub;
                perfil.LinkedIn = registro.PerfilProfesional.LinkedIn;

                // 13. Actualizar o crear las educaciones.
                foreach (var item in registro.Educaciones)
                {
                    var educacion = educacionesActuales
                        .FirstOrDefault(x => x.Id == item.Id);

                    if (item.Id == 0)
                    {
                        educacion = new Educacion
                        {
                            IdPerfilProfesional = perfil.Id
                        };

                        _context.Educaciones.Add(educacion);
                    }

                    educacion.NivelEducativo = item.NivelEducativo;
                    educacion.Institucion = item.Institucion;
                    educacion.Titulo = item.Titulo;
                    educacion.FechaInicio = item.FechaInicio;
                    educacion.FechaFin = item.FechaFin;
                    educacion.EnCurso = item.EnCurso;
                }

                // 14. Eliminar las educaciones que fueron quitadas.
                _context.Educaciones.RemoveRange(
                    educacionesActuales.Where(x => !idsEducacion.Contains(x.Id)));

                // 15. Actualizar o crear las experiencias.
                foreach (var item in registro.Experiencias)
                {
                    var experiencia = experienciasActuales
                        .FirstOrDefault(x => x.Id == item.Id);

                    if (item.Id == 0)
                    {
                        experiencia = new Experiencia
                        {
                            IdPerfilProfesional = perfil.Id
                        };

                        _context.Experiencias.Add(experiencia);
                    }

                    experiencia.Empresa = item.Empresa;
                    experiencia.Cargo = item.Cargo;
                    experiencia.Descripcion = item.Descripcion;
                    experiencia.FechaInicio = item.FechaInicio;
                    experiencia.FechaFin = item.FechaFin;
                    experiencia.EstadoActual = item.EstadoActual;
                }

                // 16. Eliminar las experiencias que fueron quitadas.
                _context.Experiencias.RemoveRange(
                    experienciasActuales.Where(x => !idsExperiencia.Contains(x.Id)));

                // 17. Actualizar o crear las certificaciones.
                foreach (var item in registro.Certificaciones)
                {
                    var certificacion = certificacionesActuales
                        .FirstOrDefault(x => x.Id == item.Id);

                    if (item.Id == 0)
                    {
                        certificacion = new Certificacion
                        {
                            IdPerfilProfesional = perfil.Id
                        };

                        _context.Certificaciones.Add(certificacion);
                    }

                    certificacion.Titulo = item.Titulo;
                    certificacion.Institucion = item.Institucion;
                    certificacion.FechaEmision = item.FechaEmision;
                    certificacion.FechaVencimiento = item.FechaVencimiento;
                    certificacion.UrlCertificado = item.UrlCertificado;
                }

                // 18. Eliminar las certificaciones que fueron quitadas.
                _context.Certificaciones.RemoveRange(
                    certificacionesActuales.Where(x => !idsCertificaciones.Contains(x.Id)));

                // 19. Sincronizar las habilidades.
                var idsHabilidadesActuales = habilidadesActuales
                    .Select(x => x.IdHabilidad)
                    .ToList();

                _context.Perfiles_Habilidades.RemoveRange(
                    habilidadesActuales.Where(x =>
                        !habilidadesSolicitadas.Contains(x.IdHabilidad)));

                foreach (var habilidadId in habilidadesSolicitadas)
                {
                    if (!idsHabilidadesActuales.Contains(habilidadId))
                    {
                        _context.Perfiles_Habilidades.Add(new Perfil_Habilidad
                        {
                            IdPerfilProfesional = perfil.Id,
                            IdHabilidad = habilidadId
                        });
                    }
                }

                // 20. Guardar y confirmar todos los cambios.
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "El perfil profesional se actualizó correctamente.",
                    idDesarrollador = desarrollador.Id,
                    idPerfilProfesional = perfil.Id
                });
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                await transaction.RollbackAsync();

                return Conflict("No se pudo actualizar el perfil porque existe información duplicada.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // DELETE /api/desarrolladores/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var desarrollador = await _context.Desarrolladores.FirstOrDefaultAsync(x => x.Id == id);
            if (desarrollador == null)
            {
                return NotFound("No existe un desarrollador con ese Id."); //404
            }

            try
            {
                _context.Remove(desarrollador);
                await _context.SaveChangesAsync();
                return NoContent(); //204
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsRelacion(ex))
            {
                return Conflict("No se puede eliminar el desarrollador porque tiene un perfil profesional o aplicaciones asociadas."); //409
            }
        }

        // Reglas que necesitan consultar la base de datos antes de guardar.
        // Devuelve el mensaje de error, o null si todo está bien.
        private Task<string> ValidarAsync(Desarrollador desarrollador)
        {
            return Task.FromResult<string>(null);
        }
    }
}
