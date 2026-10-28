using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Aplicacion
    {
        public int Id { get; set; }

        [Display(Name = "ID Publicacion")]//FK
        public int IdPublicacion { get; set; }

        [JsonIgnore]
        public Publicacion Publicacion { get; set; }


        [Display(Name = "ID Desarrollador")]//FK
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdDesarrollador { get; set; }

        [JsonIgnore]
        public Desarrollador Desarrollador { get; set; }


        [Display(Name = "Fecha de Aplicacion")] //Aqui va la fecha en la que se aplico a la publicacion
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateOnly FechaAplicacion { get; set; }


        [Display(Name = "Mensaje")] //Aqui va el mensaje que el desarrollador envia al aplicar a la publicacion
        [MaxLength(500, ErrorMessage = "El Mensaje no puede tener más de 500 caracteres")]
        public string Mensaje { get; set; }

        
        [Display(Name = "Estado")]//Aqui va el estado de la aplicacion, si esta aceptada, rechazada o en espera
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(10, ErrorMessage = "El Estado no puede tener más de 10 caracteres")]
        [RegularExpression("^(PENDIENTE|EN_REVISION|ACEPTADA|RECHAZADA|RETIRADA)$", ErrorMessage = "El estado debe ser PENDIENTE, EN_REVISION, ACEPTADA, RECHAZADA o RETIRADA")]
        public string Estado { get; set; }


        [Display(Name = "Plazo Estimado")] //Aqui va el plazo en dias que se tiene para la publicacion este solo aplica para trabajos freelance
        [Range(1, 5000, ErrorMessage = "El plazo estimado debe ser un valor válido y no negativo")]
        public int? PlazoEstimado { get; set; }


        [Display(Name = "Monto propuesto")] //Aqui va el presupuesto que se tiene para la publicacion este solo aplica para trabajos freelance
        [Range(typeof(decimal), "0", "999999999999.99", ErrorMessage = "El monto propuesto debe ser un valor válido y no negativo")]
        public decimal? MontoPropuesto { get; set; }







    }
}
