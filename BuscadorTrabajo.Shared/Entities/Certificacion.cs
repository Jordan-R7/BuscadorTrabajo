using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Certificacion
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
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



    }
}
