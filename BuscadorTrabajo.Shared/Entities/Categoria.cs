using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Categoria
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El Nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(200, ErrorMessage = "La Descripción no puede tener más de 200 caracteres")]
        public string Descripcion { get; set; }
    }
}
