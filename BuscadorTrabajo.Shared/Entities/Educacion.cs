using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Educacion
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
        public int IdPerfilProfesional { get; set; }

        [JsonIgnore]
        public PerfilProfesional PerfilProfesional { get; set; }


        [Display(Name = "Nivel Educativo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El Nivel Educativo no puede tener más de 50 caracteres")]
        public string NivelEducativo { get; set; } // Primaria, Bachillerato, Tecnico, Tecnologo, Universitario, Postgrado
        

        [Display(Name = "Institución")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "La Institución no puede tener más de 100 caracteres")]
        public string Institucion { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Título no puede tener más de 100 caracteres")]
        public string Titulo { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateOnly? FechaFin { get; set; }

        [Display(Name = "En Curso")]
        public bool EnCurso { get; set; }
    }
}
