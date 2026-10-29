using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Administrador
    {
        public int Id { get; set; }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MinLength(6, ErrorMessage = "La Cédula debe tener al menos 6 dígitos")]
        [MaxLength(10, ErrorMessage = "La Cédula no puede tener más de 10 caracteres")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cédula solo debe contener números")]
        public string Cedula { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Apellido no puede tener más de 100 caracteres")]
        public string Apellido { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El Celular no puede tener más de 20 caracteres")]
        [Phone(ErrorMessage = "El celular debe tener un formato válido")]
        public string Celular { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser una dirección de correo válida")]
        [MaxLength(300, ErrorMessage = "El Correo no puede tener más de 300 caracteres")]
        public string Correo { get; set; }
    }
}
