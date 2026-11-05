using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Para eliminar la cuenta se pide la contraseña, así nadie la borra por error
    public class EliminarCuentaDTO
    {
        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Escribe tu contraseña para confirmar.")]
        public string Password { get; set; }
    }
}
