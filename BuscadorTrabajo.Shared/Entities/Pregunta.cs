using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    // Pregunta adicional que la empresa o el cliente le hace a quien aplica
    // a una publicación (informe: tabla PREGUNTA).
    public class Pregunta : IValidatableObject
    {
        // Tipos de respuesta permitidos (RN56)
        public static readonly string[] TiposRespuesta = { "TEXTO", "SI_NO", "NUMERO", "OPCION_UNICA" };

        public int Id { get; set; }

        [Display(Name = "ID Publicación")] // FK (RN50)
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una publicación válida.")]
        public int IdPublicacion { get; set; }

        [JsonIgnore]
        public Publicacion Publicacion { get; set; }

        [Display(Name = "Pregunta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "La pregunta no puede tener más de 500 caracteres")]
        public string Texto { get; set; }

        [Display(Name = "Tipo de respuesta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El tipo de respuesta no puede tener más de 20 caracteres")]
        [RegularExpression("^(TEXTO|SI_NO|NUMERO|OPCION_UNICA)$", ErrorMessage = "El tipo de respuesta debe ser TEXTO, SI_NO, NUMERO u OPCION_UNICA")]
        public string TipoRespuesta { get; set; } = "TEXTO";

        // Solo para OPCION_UNICA: las opciones separadas por punto y coma. Ej: "Junior;Semi senior;Senior"
        [Display(Name = "Opciones")]
        [MaxLength(500, ErrorMessage = "Las opciones no pueden tener más de 500 caracteres")]
        public string Opciones { get; set; }

        [Display(Name = "Obligatoria")]
        public bool Obligatoria { get; set; } = true;

        [Display(Name = "Orden")]
        [Range(1, 255, ErrorMessage = "El orden debe estar entre 1 y 255")]
        public byte Orden { get; set; } = 1;

        // Lista de opciones ya separadas (no se guarda en la base de datos)
        public List<string> ListaOpciones() =>
            string.IsNullOrWhiteSpace(Opciones)
                ? new List<string>()
                : Opciones.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoRespuesta == "OPCION_UNICA")
            {
                if (ListaOpciones().Count < 2)
                {
                    yield return new ValidationResult(
                        "Una pregunta de opción única necesita al menos dos opciones separadas por punto y coma (;).",
                        new[] { nameof(Opciones) });
                }
            }
            else if (!string.IsNullOrWhiteSpace(Opciones))
            {
                yield return new ValidationResult(
                    "Las opciones solo se usan en preguntas de opción única.",
                    new[] { nameof(Opciones) });
            }
        }
    }
}
