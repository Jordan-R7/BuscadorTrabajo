using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Experiencia : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un perfil profesional válido.")]
        public int IdPerfilProfesional { get; set; }

        [JsonIgnore]
        public PerfilProfesional PerfilProfesional { get; set; }

        [Display(Name = "Empresa")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "La Empresa no puede tener más de 100 caracteres")]
        public string Empresa { get; set; }

        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Cargo no puede tener más de 100 caracteres")]
        public string Cargo { get; set; }

        [Display(Name = "Descripción")] 
        [MaxLength(250, ErrorMessage = "La Descripción no puede tener más de 250 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateOnly? FechaFin { get; set; }

        [Display(Name = "Estado Actual")]
        public bool EstadoActual { get; set; }

        // Reglas que comparan varios campos
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // RN10: la fecha de inicio es obligatoria y no puede ser futura
            if (FechaInicio == default)
            {
                yield return new ValidationResult("La fecha de inicio es obligatoria.", new[] { nameof(FechaInicio) });
            }
            else if (FechaInicio > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult("La fecha de inicio no puede ser posterior a hoy.", new[] { nameof(FechaInicio) });
            }

            // RN10: la fecha de fin no puede ser anterior a la de inicio
            if (FechaFin != null && FechaFin < FechaInicio)
            {
                yield return new ValidationResult("La fecha de fin no puede ser anterior a la fecha de inicio.", new[] { nameof(FechaFin) });
            }

            // RN11: si es el trabajo actual, no lleva fecha de fin
            if (EstadoActual && FechaFin != null)
            {
                yield return new ValidationResult("Si es el trabajo actual, la fecha de fin debe quedar vacía.", new[] { nameof(FechaFin) });
            }
        }
    }
}
