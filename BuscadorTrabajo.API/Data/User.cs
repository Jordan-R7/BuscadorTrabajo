using System.ComponentModel.DataAnnotations;
using BuscadorTrabajo.Shared.Enums;
using Microsoft.AspNetCore.Identity;

namespace BuscadorTrabajo.API.Data
{
    // Usuario del sistema (informe: tabla USUARIO).
    // Hereda de IdentityUser, que ya trae: Id, Email, UserName, PasswordHash, PhoneNumber...
    // Aquí se agregan los campos propios. Vive en el API porque la página web nunca lo
    // usa directamente: la web solo recibe el token y los DTO.
    public class User : IdentityUser
    {
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(100, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        public string Apellido { get; set; }

        [Display(Name = "Tipo de usuario")]
        public UserType TipoUsuario { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Display(Name = "Último acceso")]
        public DateTime? UltimoAcceso { get; set; }

        // Id de la foto de perfil (tabla Imagenes). NULL = el usuario no tiene foto.
        [Display(Name = "Foto")]
        public Guid? FotoId { get; set; }

        public string NombreCompleto => string.IsNullOrWhiteSpace(Apellido) ? Nombre : $"{Nombre} {Apellido}";
    }
}
