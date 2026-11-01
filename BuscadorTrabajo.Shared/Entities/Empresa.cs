using System.ComponentModel.DataAnnotations;
using BuscadorTrabajo.Shared.Validaciones;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Empresa
    {
       
        
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El Nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "NIT")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(12, ErrorMessage = "El NIT no puede tener más de 12 caracteres")]
        public string NIT { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "La Descripcion no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Pais")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El Pais no puede tener más de 50 caracteres")]
        public string Pais { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "La Ciudad no puede tener más de 50 caracteres")]
        public string Ciudad { get; set; }

        [Display(Name = "Direccion")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(150, ErrorMessage = "La Direccion no puede tener más de 150 caracteres")]
        public string Direccion { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El Celular no puede tener más de 20 caracteres")]
        [Phone(ErrorMessage = "El celular debe tener un formato válido")]
        public string Celular { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(300, ErrorMessage = "El Correo no puede tener más de 300 caracteres")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
        public string Correo { get; set; }

        [Display(Name = "Sitio Web")]
        [MaxLength(300, ErrorMessage = "El Sitio Web no puede tener más de 300 caracteres")]
        [UrlOpcional(ErrorMessage = "El sitio web debe ser una URL válida")]
        public string SitioWeb { get; set; }

        
        
    }
}
