using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class PerfilProfesional
    {
        public int Id { get; set; }

        [Display(Name = "ID Desarrollador")] //FK
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un desarrollador válido.")]
        public int IdDesarrollador { get; set; }

        [JsonIgnore]
        public Desarrollador Desarrollador { get; set; }

        [Display(Name = "Título Profesional")] //Aqui va el titulo profesional del desarrollador, por ejemplo: Ingeniero en Sistemas.
        [MaxLength(100, ErrorMessage = "El Título Profesional no puede tener más de 100 caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string TituloProfesional { get; set; }

        [Display(Name = "Descripción")] //Aqui va una breve descripcion del perfil profesional del desarrollador
        [MaxLength(500, ErrorMessage = "La Descripción no puede tener más de 500 caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Descripcion { get; set; }

        [Display(Name = "País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(30, ErrorMessage = "El País no puede tener más de 30 caracteres")]
        public string Pais { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(30, ErrorMessage = "La Ciudad no puede tener más de 30 caracteres")]
        public string Ciudad { get; set; }

        [Display(Name = "Modalidad")] //Aqui va la modalidad de trabajo que el desarrollador prefiere, si es remoto, presencial o híbrido
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [RegularExpression("^(REMOTO|HIBRIDO|PRESENCIAL)$", ErrorMessage = "La modalidad debe ser REMOTO, HIBRIDO o PRESENCIAL")] // Con esto podemos hacer la lista despegable
        public string Modalidad { get; set; }

        [Display(Name = "GitHub")]
        [MaxLength(300, ErrorMessage = "El GitHub no puede tener más de 300 caracteres")]
        [Url(ErrorMessage = "El sitio web debe ser una URL válida")]
        public string GitHub { get; set; }

        [Display(Name = "LinkedIn")]
        [MaxLength(300, ErrorMessage = "El LinkedIn no puede tener más de 300 caracteres")]
        [Url(ErrorMessage = "El sitio web debe ser una URL válida")]
        public string LinkedIn { get; set; }


    }
}
