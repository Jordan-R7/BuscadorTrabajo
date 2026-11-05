using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using BuscadorTrabajo.Shared.DTOs;
using BuscadorTrabajo.Shared.Entities;
using BuscadorTrabajo.Shared.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BuscadorTrabajo.API.Controllers
{
    // Cuentas de usuario: registro, inicio de sesión y datos del usuario conectado.
    [ApiController]
    [Route("/api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration, DataContext context)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _context = context;
        }

        // POST /api/accounts/CreateUser
        // Crea la cuenta (Empresa, Cliente o Administrador) y su registro vinculado.
        // Solo un administrador conectado puede crear otra cuenta de administrador.
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] RegistroUsuarioDTO model)
        {
            if (model.TipoUsuario == UserType.Admin && !User.IsInRole(nameof(UserType.Admin)))
            {
                // El token se lee aunque el endpoint sea público: si no es admin, no puede crear admins
                return Forbid(JwtBearerDefaults.AuthenticationScheme); //403
            }

            if (model.TipoUsuario == UserType.Desarrollador)
            {
                return BadRequest("Los desarrolladores se registran con el asistente de registro de desarrollador."); //400
            }

            var email = model.Email.Trim().ToLower();
            if (await _userHelper.GetUserAsync(email) != null)
            {
                return Conflict("Ya existe una cuenta con ese correo."); //409
            }

            // 1. Se arma el registro del rol y se valida con las mismas reglas de su entidad
            object registro = CrearRegistro(model, email);
            var errores = new List<ValidationResult>();
            if (!Validator.TryValidateObject(registro, new ValidationContext(registro), errores, true))
            {
                return BadRequest(errores.First().ErrorMessage); //400
            }

            // 2. Usuario y registro se guardan juntos: si algo falla no queda nada a medias
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Email = email,
                    UserName = email,
                    PhoneNumber = model.Celular,
                    Nombre = model.TipoUsuario == UserType.Empresa ? model.NombreEmpresa : model.Nombre,
                    Apellido = model.TipoUsuario == UserType.Empresa ? null : model.Apellido,
                    TipoUsuario = model.TipoUsuario,
                    UltimoAcceso = DateTime.UtcNow
                };

                var result = await _userHelper.AddUserAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(TraducirError(result.Errors.FirstOrDefault())); //400
                }

                await _userHelper.AddUserToRoleAsync(user, user.TipoUsuario.ToString());

                switch (registro)
                {
                    case Empresa empresa: empresa.UserId = user.Id; break;
                    case Cliente cliente: cliente.UserId = user.Id; break;
                    case Administrador administrador: administrador.UserId = user.Id; break;
                }

                _context.Add(registro);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(await BuildTokenAsync(user)); //200 + token: queda con la sesión iniciada
            }
            catch (DbUpdateException ex) when (ErroresBaseDatos.EsDuplicado(ex))
            {
                await transaction.RollbackAsync();
                return Conflict(model.TipoUsuario == UserType.Empresa
                    ? "Ya existe una empresa registrada con ese NIT."
                    : "Ya existe un registro con esa cédula."); //409
            }
        }

        // POST /api/accounts/Login
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
            model.Email = model.Email.Trim().ToLower();
            var result = await _userHelper.LoginAsync(model);

            if (result.IsLockedOut)
            {
                return BadRequest("Superaste el número de intentos. Tu cuenta quedó bloqueada unos minutos."); //400
            }

            if (!result.Succeeded)
            {
                return BadRequest("Correo o contraseña incorrectos."); //400
            }

            var user = await _userHelper.GetUserAsync(model.Email);
            if (!user.Activo || !await RegistroActivoAsync(user))
            {
                return BadRequest("Tu cuenta está desactivada. Comunícate con un administrador."); //400
            }

            user.UltimoAcceso = DateTime.UtcNow;
            await _userHelper.UpdateUserAsync(user);

            return Ok(await BuildTokenAsync(user));
        }

        // GET /api/accounts/me  ->  datos del usuario conectado (página "Mi cuenta")
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Me()
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            return Ok(new CuentaDTO
            {
                Email = user.Email,
                TipoUsuario = user.TipoUsuario.ToString(),
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Celular = user.PhoneNumber,
                FotoId = user.FotoId,
                FechaRegistro = user.FechaRegistro,
                UltimoAcceso = user.UltimoAcceso
            });
        }

        // PUT /api/accounts  ->  el usuario cambia su nombre, apellido y celular.
        // También se actualiza su registro vinculado (desarrollador, empresa, cliente o administrador).
        // Devuelve un token nuevo porque el nombre viaja dentro del token.
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put([FromBody] CuentaDTO model)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            var esEmpresa = user.TipoUsuario == UserType.Empresa;
            if (!esEmpresa && string.IsNullOrWhiteSpace(model.Apellido))
            {
                return BadRequest("El apellido es obligatorio."); //400
            }

            user.Nombre = model.Nombre.Trim();
            user.Apellido = esEmpresa ? null : model.Apellido.Trim();
            user.PhoneNumber = model.Celular.Trim();

            // Registro vinculado: se cambian los mismos datos y se valida con las reglas de su entidad
            object registro = user.TipoUsuario switch
            {
                UserType.Desarrollador => await _context.Desarrolladores.FirstOrDefaultAsync(x => x.UserId == user.Id),
                UserType.Empresa => await _context.Empresas.FirstOrDefaultAsync(x => x.UserId == user.Id),
                UserType.Cliente => await _context.Clientes.FirstOrDefaultAsync(x => x.UserId == user.Id),
                _ => await _context.Administradores.FirstOrDefaultAsync(x => x.UserId == user.Id)
            };

            switch (registro)
            {
                case Desarrollador d: d.Nombre = user.Nombre; d.Apellido = user.Apellido; d.Celular = user.PhoneNumber; break;
                case Empresa e: e.Nombre = user.Nombre; e.Celular = user.PhoneNumber; break;
                case Cliente c: c.Nombre = user.Nombre; c.Apellido = user.Apellido; c.Celular = user.PhoneNumber; break;
                case Administrador a: a.Nombre = user.Nombre; a.Apellido = user.Apellido; a.Celular = user.PhoneNumber; break;
            }

            if (registro != null)
            {
                var errores = new List<ValidationResult>();
                if (!Validator.TryValidateObject(registro, new ValidationContext(registro), errores, true))
                {
                    return BadRequest(errores.First().ErrorMessage); //400
                }
            }

            await _context.SaveChangesAsync();      // guarda el registro vinculado
            await _userHelper.UpdateUserAsync(user); // guarda el usuario
            return Ok(await BuildTokenAsync(user));
        }

        // PUT /api/accounts/foto  ->  sube o cambia la foto de perfil (PNG, JPG o JPEG, máximo 2 MB).
        // La imagen se guarda en la tabla Imagenes con un Id nuevo y ese Id queda en el usuario (FotoId).
        [HttpPut("foto")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> PutFoto([FromBody] FotoDTO model)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            // 1. Extensión permitida
            var extension = Path.GetExtension(model.NombreArchivo ?? "").ToLowerInvariant();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
            {
                return BadRequest("Solo se permiten imágenes PNG, JPG o JPEG."); //400
            }

            // 2. Convertir el texto Base64 en bytes
            byte[] datos;
            try
            {
                datos = Convert.FromBase64String(model.Base64);
            }
            catch (FormatException)
            {
                return BadRequest("La imagen no es válida."); //400
            }

            if (datos.Length == 0 || datos.Length > TamanoMaximoFoto)
            {
                return BadRequest("La imagen debe pesar máximo 2 MB."); //400
            }

            // 3. Revisar el contenido real del archivo (no basta con el nombre)
            var tipo = TipoImagen(datos);
            if (tipo == null)
            {
                return BadRequest("El archivo no es una imagen PNG o JPEG válida."); //400
            }

            // 4. Guardar la imagen nueva, apuntar el usuario a ella y borrar la anterior
            var anterior = user.FotoId;
            var imagen = new Imagen
            {
                Id = Guid.NewGuid(),
                NombreArchivo = Path.GetFileName(model.NombreArchivo),
                TipoContenido = tipo,
                Datos = datos,
                Tamano = datos.Length
            };

            _context.Imagenes.Add(imagen);
            user.FotoId = imagen.Id;
            await _context.SaveChangesAsync();

            if (anterior != null)
            {
                await _context.Imagenes.Where(x => x.Id == anterior).ExecuteDeleteAsync();
            }

            // Token nuevo: el Id de la foto viaja en el token para mostrarla en la barra superior
            return Ok(await BuildTokenAsync(user));
        }

        // DELETE /api/accounts/foto  ->  quita la foto de perfil (FotoId vuelve a NULL)
        [HttpDelete("foto")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteFoto()
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            var anterior = user.FotoId;
            user.FotoId = null;
            await _context.SaveChangesAsync();

            if (anterior != null)
            {
                await _context.Imagenes.Where(x => x.Id == anterior).ExecuteDeleteAsync();
            }

            return Ok(await BuildTokenAsync(user));
        }

        // POST /api/accounts/eliminar  ->  el usuario elimina su propia cuenta (pide la contraseña).
        // Qué pasa con su registro:
        //  - Desarrollador con aplicaciones: queda desactivado para no perder el historial; sin aplicaciones se borra con su perfil.
        //  - Empresa o cliente con publicaciones: sus publicaciones abiertas se cancelan y el registro se conserva
        //    (las publicaciones y aplicaciones no se pierden); sin publicaciones se borra.
        //  - Administrador: no se puede eliminar el último administrador del sistema.
        [HttpPost("eliminar")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Eliminar([FromBody] EliminarCuentaDTO model)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            if (!await _userHelper.CheckPasswordAsync(user, model.Password))
            {
                return BadRequest("La contraseña no es correcta."); //400
            }

            if (user.TipoUsuario == UserType.Admin && await _userHelper.CountUsersInRoleAsync(nameof(UserType.Admin)) <= 1)
            {
                return BadRequest("No puedes eliminar la cuenta del único administrador del sistema."); //400
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            switch (user.TipoUsuario)
            {
                case UserType.Desarrollador:
                    var desarrollador = await _context.Desarrolladores.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    if (desarrollador != null)
                    {
                        desarrollador.UserId = null;
                        if (await _context.Aplicaciones.AnyAsync(x => x.IdDesarrollador == desarrollador.Id))
                        {
                            desarrollador.Activo = false;
                        }
                        else
                        {
                            // Su perfil se borra con experiencias, educación, certificaciones y habilidades (cascada)
                            var perfil = await _context.PerfilesProfesionales.FirstOrDefaultAsync(x => x.IdDesarrollador == desarrollador.Id);
                            if (perfil != null)
                            {
                                _context.Remove(perfil);
                            }
                            _context.Remove(desarrollador);
                        }
                    }
                    break;

                case UserType.Empresa:
                case UserType.Cliente:
                    var esEmpresa = user.TipoUsuario == UserType.Empresa;
                    object registro = esEmpresa
                        ? await _context.Empresas.FirstOrDefaultAsync(x => x.UserId == user.Id)
                        : await _context.Clientes.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    if (registro != null)
                    {
                        var idRegistro = registro is Empresa e ? e.Id : ((Cliente)registro).Id;
                        var publicaciones = await _context.Publicaciones
                            .Where(x => esEmpresa ? x.IdEmpresa == idRegistro : x.IdCliente == idRegistro)
                            .ToListAsync();

                        if (publicaciones.Count > 0)
                        {
                            foreach (var publicacion in publicaciones.Where(x => x.Estado != "CERRADA" && x.Estado != "CANCELADA"))
                            {
                                publicacion.Estado = "CANCELADA";
                            }

                            if (registro is Empresa empresa) { empresa.UserId = null; } else { ((Cliente)registro).UserId = null; }
                        }
                        else
                        {
                            _context.Remove(registro);
                        }
                    }
                    break;

                default:
                    var administrador = await _context.Administradores.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    if (administrador != null)
                    {
                        _context.Remove(administrador);
                    }
                    break;
            }

            var foto = user.FotoId;
            user.FotoId = null;
            await _context.SaveChangesAsync();

            if (foto != null)
            {
                await _context.Imagenes.Where(x => x.Id == foto).ExecuteDeleteAsync();
            }

            var resultado = await _userHelper.DeleteUserAsync(user);
            if (!resultado.Succeeded)
            {
                await transaction.RollbackAsync();
                return BadRequest("No se pudo eliminar la cuenta."); //400
            }

            await transaction.CommitAsync();
            return NoContent(); //204
        }

        // Tamaño máximo de la foto de perfil: 2 MB
        private const int TamanoMaximoFoto = 2 * 1024 * 1024;

        // Reconoce el tipo por los primeros bytes del archivo: PNG empieza con 89 50 4E 47 y JPEG con FF D8 FF
        private static string TipoImagen(byte[] datos)
        {
            if (datos.Length > 8 && datos[0] == 0x89 && datos[1] == 0x50 && datos[2] == 0x4E && datos[3] == 0x47)
            {
                return "image/png";
            }

            if (datos.Length > 3 && datos[0] == 0xFF && datos[1] == 0xD8 && datos[2] == 0xFF)
            {
                return "image/jpeg";
            }

            return null;
        }

        // POST /api/accounts/changePassword
        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePassword([FromBody] CambioClaveDTO model)
        {
            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound("El usuario ya no existe."); //404
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.ClaveActual, model.ClaveNueva);
            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                return BadRequest(error?.Code == "PasswordMismatch"
                    ? "La contraseña actual no es correcta."
                    : TraducirError(error)); //400
            }

            return NoContent(); //204
        }

        // Arma la Empresa, el Cliente o el Administrador a partir del formulario de registro
        private static object CrearRegistro(RegistroUsuarioDTO model, string email)
        {
            if (model.TipoUsuario == UserType.Empresa)
            {
                return new Empresa
                {
                    Nombre = model.NombreEmpresa?.Trim(),
                    NIT = model.NIT?.Trim(),
                    Descripcion = model.Descripcion?.Trim(),
                    Pais = model.Pais?.Trim(),
                    Ciudad = model.Ciudad?.Trim(),
                    Direccion = model.Direccion?.Trim(),
                    Celular = model.Celular?.Trim(),
                    Correo = email,
                    SitioWeb = string.IsNullOrWhiteSpace(model.SitioWeb) ? null : model.SitioWeb.Trim()
                };
            }

            if (model.TipoUsuario == UserType.Admin)
            {
                return new Administrador
                {
                    Cedula = model.Cedula?.Trim(),
                    Nombre = model.Nombre?.Trim(),
                    Apellido = model.Apellido?.Trim(),
                    Celular = model.Celular?.Trim(),
                    Correo = email
                };
            }

            return new Cliente
            {
                Cedula = model.Cedula?.Trim(),
                Nombre = model.Nombre?.Trim(),
                Apellido = model.Apellido?.Trim(),
                Celular = model.Celular?.Trim(),
                Correo = email
            };
        }

        // Id del registro (Desarrollador, Empresa, Cliente o Administrador) vinculado al usuario
        private async Task<int> IdRegistroAsync(User user)
        {
            return user.TipoUsuario switch
            {
                UserType.Desarrollador => await _context.Desarrolladores.Where(x => x.UserId == user.Id).Select(x => x.Id).FirstOrDefaultAsync(),
                UserType.Empresa => await _context.Empresas.Where(x => x.UserId == user.Id).Select(x => x.Id).FirstOrDefaultAsync(),
                UserType.Cliente => await _context.Clientes.Where(x => x.UserId == user.Id).Select(x => x.Id).FirstOrDefaultAsync(),
                _ => await _context.Administradores.Where(x => x.UserId == user.Id).Select(x => x.Id).FirstOrDefaultAsync()
            };
        }

        // Un desarrollador desactivado por el administrador no puede entrar
        private async Task<bool> RegistroActivoAsync(User user)
        {
            if (user.TipoUsuario != UserType.Desarrollador)
            {
                return true;
            }

            return await _context.Desarrolladores.AnyAsync(x => x.UserId == user.Id && x.Activo);
        }

        // Crea el token JWT con los datos (claims) del usuario. Vence en 30 días.
        private async Task<TokenDTO> BuildTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.TipoUsuario.ToString()),
                new Claim("Nombre", user.Nombre ?? string.Empty),
                new Claim("Apellido", user.Apellido ?? string.Empty),
                new Claim("NombreCompleto", user.NombreCompleto ?? string.Empty),
                new Claim("IdRegistro", (await IdRegistroAsync(user)).ToString()),
                new Claim("FotoId", user.FotoId?.ToString() ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(30);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }

        // Los errores de Identity vienen en inglés: se traducen los más comunes
        private static string TraducirError(IdentityError error)
        {
            return error?.Code switch
            {
                "DuplicateUserName" or "DuplicateEmail" => "Ya existe una cuenta con ese correo.",
                "InvalidEmail" => "El correo no es válido.",
                "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
                _ => error?.Description ?? "No se pudo crear el usuario."
            };
        }
    }
}
