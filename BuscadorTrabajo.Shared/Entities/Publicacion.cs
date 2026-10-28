using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Publicacion
    {
        public int Id { get; set; }

        
        [Display(Name = "ID Empresa")]//FK
        public int? IdEmpresa { get; set; }

        [JsonIgnore]
        public Empresa Empresa { get; set; }

        
        [Display(Name = "ID Cliente")]//FK
        public int? IdCliente { get; set; }

        [JsonIgnore]
        public Cliente Cliente { get; set; }


        
        [Display(Name = "ID Categoria")]//FK
        public int IdCategoria { get; set; }

        [JsonIgnore]
        public Categoria Categoria { get; set; }


        [Display(Name = "Titulo")] //Aqui va el nombre del trabajo que se va a publicar
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El título no puede tener más de 50 caracteres")]
        public string Titulo { get; set; }


        [Display(Name = "Descripcion")] //Una breve descripcion del trabajo que se va a publicar
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Tipo")] //Aqui va el tipo de publicacion, si es freelance o tiempo completo
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El tipo no puede tener más de 50 caracteres")]
        public string Tipo { get; set; } 


        [Display(Name = "Estado")] //Aqui va el estado de la publicacion, si esta activa o inactiva
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El estado no puede tener más de 20 caracteres")]
        [RegularExpression("^(CREADA|PUBLICADA|VENCIDA|OCULTA)$", ErrorMessage = "El estado debe ser CREADA, PUBLICADA, VENCIDA o OCULTA")]
        public string Estado { get; set; }


        [Display(Name = "Fecha de Creacion")] //Aqui va la fecha en la que se creo la publicacion
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateOnly FechaCreacion { get; set; }


        [Display(Name = "Fecha de Publicacion")] //Aqui va la fecha en la que se publico la publicacion
        public DateOnly? FechaPublicacion { get; set; }


        [Display(Name = "Fecha de Vencimiento")] //Aqui va la fecha en la que se vencera la publicacion
        public DateOnly? FechaVencimiento { get; set; }


        [Display(Name = "Presupuesto")] //Aqui va el presupuesto que se tiene para la publicacion este solo aplica para trabajos freelance
        [Range(typeof(decimal), "0", "999999999999.99",ErrorMessage = "El presupuesto debe ser un valor válido y no negativo")]
        public decimal? Presupuesto { get; set; }


        [Display(Name = "Salario")] //Aqui va el salario que se tiene para la publicacion este solo aplica para trabajos de tiempo completo
        [Range(typeof(decimal), "0", "999999999999.99", ErrorMessage = "El salario debe ser un valor válido y no negativo")]
        public decimal? Salario { get; set; }


        [Display(Name = "Moneda")] //Aqui va la moneda en la que se tiene el presupuesto o salario de la publicacion
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(5, ErrorMessage = "La moneda no puede tener más de 5 caracteres")]
        public string Moneda { get; set; }


        [Display(Name = "Nivel de Experiencia")] //Aqui va el nivel de experiencia que se requiere para la publicacion este solo aplica para trabajos de tiempo completo
        [MaxLength(60, ErrorMessage = "El nivel de experiencia no puede tener más de 60 caracteres")]
        public string NivelExperiencia { get; set; }


        [Display(Name = "Tipo de Contrato")] //Aqui va el tipo de contrato que se tiene para la publicacion este solo aplica para trabajos de tiempo completo
        [MaxLength(50, ErrorMessage = "El tipo de contrato no puede tener más de 50 caracteres")]
        public string TipoContrato { get; set; }


        [Display(Name = "Modalidad de Trabajo")] //Aqui va la modalidad de trabajo que se tiene para la publicacion, puede ser presencial, remoto o hibrido este solo aplica para trabajos de tiempo completo
        [MaxLength(20, ErrorMessage = "La modalidad de trabajo no puede tener más de 20 caracteres")]
        public string ModalidadTrabajo { get; set; }


        [Display(Name = "Pais")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "El país no puede tener más de 50 caracteres")]
        public string Pais { get; set; }


        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(50, ErrorMessage = "La ciudad no puede tener más de 50 caracteres")]
        public string Ciudad { get; set; }


        [Display(Name = "Plazo")] //Aqui va el plazo en dias que se tiene para la publicacion este solo aplica para trabajos freelance
        [Range(1, 5000, ErrorMessage = "El plazo debe ser un valor válido y no negativo")]
        public int? Plazo { get; set; }




    }
}
