using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BuscadorTrabajo.Shared.Validaciones;
using System.Text;

namespace BuscadorTrabajo.Shared.DTOs
{
    public class RegistroDesarrolladorDTO
    {
        [Required]
        public DesarrolladorRegistroDTO DatosPersonales { get; set; } = new();

        [Required]
        public PerfilProfesionalRegistroDTO PerfilProfesional { get; set; } = new();

        public List<EducacionRegistroDTO> Educaciones { get; set; } = new();

        public List<ExperienciaRegistroDTO> Experiencias { get; set; } = new();

        public List<CertificacionRegistroDTO> Certificaciones { get; set; } = new();

        public List<int> HabilidadesIds { get; set; } = new();

    }

    public class DesarrolladorRegistroDTO : IValidatableObject
    {
      

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [MinLength(6, ErrorMessage = "La cédula debe tener al menos 6 dígitos.")]
        [MaxLength(10, ErrorMessage = "La cédula no puede superar los 10 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cédula solo debe contener números.")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El celular es obligatorio.")]
        [MaxLength(20)]
        [Phone(ErrorMessage = "El celular no tiene un formato válido.")]
        public string Celular { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [MaxLength(300)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        public DateOnly FechaNacimiento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaNacimiento == default)
            {
                yield return new ValidationResult(
                    "La fecha de nacimiento es obligatoria.",
                    new[] { nameof(FechaNacimiento) });
            }
            else if (FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "La fecha de nacimiento no puede ser futura.",
                    new[] { nameof(FechaNacimiento) });
            }
        }
    }

    public class PerfilProfesionalRegistroDTO
    {
        [Required(ErrorMessage = "El título profesional es obligatorio.")]
        [MaxLength(100)]
        public string TituloProfesional { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [MaxLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El país es obligatorio.")]
        [MaxLength(30)]
        public string Pais { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [MaxLength(30)]
        public string Ciudad { get; set; }

        [Required(ErrorMessage = "La modalidad es obligatoria.")]
        [RegularExpression(
            "^(REMOTO|HIBRIDO|PRESENCIAL)$",
            ErrorMessage = "Selecciona una modalidad válida.")]
        public string Modalidad { get; set; }

        [MaxLength(300)]
        [UrlOpcional(ErrorMessage = "El enlace de GitHub no es válido.")]
        public string GitHub { get; set; }

        [MaxLength(300)]
        [UrlOpcional(ErrorMessage = "El enlace de LinkedIn no es válido.")]
        public string LinkedIn { get; set; }
    }

        
    public class EducacionRegistroDTO : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nivel educativo es obligatorio.")]
        [MaxLength(50)]
        public string NivelEducativo { get; set; }

        [Required(ErrorMessage = "La institución es obligatoria.")]
        [MaxLength(100)]
        public string Institucion { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(100)]
        public string Titulo { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public bool EnCurso { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(FechaInicio == default)
{
                yield return new ValidationResult(
                    "La fecha de inicio es obligatoria.",
                    new[] { nameof(FechaInicio) });
            }
                else if (FechaInicio > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser posterior a hoy.",
                    new[] { nameof(FechaInicio) });
            }

            if (FechaFin != null && FechaFin < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización no puede ser anterior al inicio.",
                    new[] { nameof(FechaFin) });
            }

            if (EnCurso && FechaFin != null)
            {
                yield return new ValidationResult(
                    "Si los estudios están en curso, no debes indicar una fecha de finalización.",
                    new[] { nameof(FechaFin) });
            }
        }
    }

    
    public class ExperienciaRegistroDTO : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La empresa es obligatoria.")]
        [MaxLength(100)]
        public string Empresa { get; set; }

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        [MaxLength(100)]
        public string Cargo { get; set; }

        [MaxLength(250)]
        public string Descripcion { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public bool EstadoActual { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaInicio == default)
            {
                yield return new ValidationResult(
                    "La fecha de inicio es obligatoria.",
                    new[] { nameof(FechaInicio) });
            }
            else if (FechaInicio > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser futura.",
                    new[] { nameof(FechaInicio) });
            }

            if (FechaFin != null && FechaFin < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de finalización no puede ser anterior al inicio.",
                    new[] { nameof(FechaFin) });
            }

            if (EstadoActual && FechaFin != null)
            {
                yield return new ValidationResult(
                    "Si es tu trabajo actual, no debes indicar una fecha de finalización.",
                    new[] { nameof(FechaFin) });
            }
        }
    }

    public class CertificacionRegistroDTO : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título de la certificación es obligatorio.")]
        [MaxLength(100)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La institución es obligatoria.")]
        [MaxLength(100)]
        public string Institucion { get; set; }

        public DateOnly FechaEmision { get; set; }

        public DateOnly? FechaVencimiento { get; set; }

        [MaxLength(300)]
        [UrlOpcional(ErrorMessage = "El enlace del certificado no es válido.")]
        public string UrlCertificado { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaEmision == default)
            {
                yield return new ValidationResult(
                    "La fecha de emisión es obligatoria.",
                    new[] { nameof(FechaEmision) });
            }

            if (FechaVencimiento != null && FechaVencimiento < FechaEmision)
            {
                yield return new ValidationResult(
                    "El vencimiento no puede ser anterior a la emisión.",
                    new[] { nameof(FechaVencimiento) });
            }
        }
    }
}
