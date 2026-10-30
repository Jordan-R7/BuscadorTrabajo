using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Habilidad
    {
        public int Id { get; set; }


        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El Nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }


        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "La Descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }


        [Display(Name = "IdCategoría")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int IdCategoria { get; set; }

        [JsonIgnore]
        public Categoria Categoria { get; set; }
    }
}
