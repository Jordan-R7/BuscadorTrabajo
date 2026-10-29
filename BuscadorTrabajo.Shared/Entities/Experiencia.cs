using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Experiencia
    {
        public int Id { get; set; }

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
    }
}
