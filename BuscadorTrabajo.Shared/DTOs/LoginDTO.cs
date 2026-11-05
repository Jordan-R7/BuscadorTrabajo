using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Datos que envía la pantalla de inicio de sesión.
    public class LoginDTO
    {
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres")]
        public string Password { get; set; }
    }
}
