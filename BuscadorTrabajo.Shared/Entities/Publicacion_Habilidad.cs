using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Publicacion_Habilidad
    {
        [Display(Name = "Id Publicacion")]//FK]
        public int IdPublicacion { get; set; }
        [JsonIgnore]
        public Publicacion Publicacion { get; set; }

        [Display(Name = "Id Habilidad")]//FK]
        public int IdHabilidad { get; set; }
        [JsonIgnore]
        public Habilidad Habilidad { get; set; }
    }
}
