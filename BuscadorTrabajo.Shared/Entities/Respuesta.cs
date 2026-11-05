using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    // Respuesta de un desarrollador a una pregunta, dentro de su aplicación
    // (informe: tabla RESPUESTA). Se envía junto con la aplicación.
    public class Respuesta
    {
        public int Id { get; set; }

        // FK. Al aplicar todavía no existe la aplicación: el Id lo pone la base de datos (RN52)
        [Display(Name = "ID Aplicación")]
        public int IdAplicacion { get; set; }

        [JsonIgnore]
        public Aplicacion Aplicacion { get; set; }

        [Display(Name = "ID Pregunta")] // FK (RN52)
        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar una pregunta válida.")]
        public int IdPregunta { get; set; }

        [JsonIgnore]
        public Pregunta Pregunta { get; set; }

        [Display(Name = "Respuesta")]
        [MaxLength(1000, ErrorMessage = "La respuesta no puede tener más de 1000 caracteres")]
        public string Valor { get; set; }

        [Display(Name = "Fecha de respuesta")]
        public DateTime FechaRespuesta { get; set; }
    }
}
