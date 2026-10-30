using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Certificacion : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un perfil profesional válido.")]
        public int IdPerfilProfesional { get; set; }

        [JsonIgnore]
        public PerfilProfesional PerfilProfesional { get; set; }

        [Display(Name = "Titulo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El Titulo no puede tener más de 100 caracteres")]
        public string Titulo { get; set; }

        [Display(Name = "Institucion")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "La Institucion no puede tener más de 100 caracteres")]
        public string Institucion { get; set; }

        [Display(Name = "Fecha de Emision")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly FechaEmision { get; set; }

        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateOnly? FechaVencimiento { get; set; }

        [Display(Name = "Url del Certificado ")]
        [MaxLength(300, ErrorMessage = "La Url del Certificado no puede tener más de 300 caracteres")]
        public string UrlCertificado { get; set; }

        // Reglas que comparan varios campos
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaEmision == default)
            {
                yield return new ValidationResult("La fecha de emisión es obligatoria.", new[] { nameof(FechaEmision) });
            }

            // RN12: la fecha de vencimiento no puede ser anterior a la de emisión
            if (FechaVencimiento != null && FechaVencimiento < FechaEmision)
            {
                yield return new ValidationResult("La fecha de vencimiento no puede ser anterior a la fecha de emisión.", new[] { nameof(FechaVencimiento) });
            }
        }
    }
}
