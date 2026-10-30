using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Educacion : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un perfil profesional válido.")]
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

        // Reglas que comparan varios campos
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // RN10: la fecha de inicio es obligatoria
            if (FechaInicio == default)
            {
                yield return new ValidationResult("La fecha de inicio es obligatoria.", new[] { nameof(FechaInicio) });
            }

            // RN10: la fecha de fin no puede ser anterior a la de inicio
            if (FechaFin != null && FechaFin < FechaInicio)
            {
                yield return new ValidationResult("La fecha de fin no puede ser anterior a la fecha de inicio.", new[] { nameof(FechaFin) });
            }

            // RN11: si está en curso, no lleva fecha de fin
            if (EnCurso && FechaFin != null)
            {
                yield return new ValidationResult("Si la formación está en curso, la fecha de fin debe quedar vacía.", new[] { nameof(FechaFin) });
            }
        }
    }
}
