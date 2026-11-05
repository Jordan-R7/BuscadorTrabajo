using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Cambio de contraseña del usuario conectado
    public class CambioClaveDTO
    {
        [Display(Name = "Contraseña actual")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        public string ClaveActual { get; set; }

        [Display(Name = "Nueva contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres")]
        [DataType(DataType.Password)]
        public string ClaveNueva { get; set; }

        [Display(Name = "Confirmación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Compare(nameof(ClaveNueva), ErrorMessage = "La nueva contraseña y la confirmación no son iguales")]
        [DataType(DataType.Password)]
        public string Confirmacion { get; set; }
    }
}
