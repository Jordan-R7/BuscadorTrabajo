using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Publicacion_Habilidad
    {
        public int Id { get; set; }

        [Display(Name = "Id Publicacion")]//FK]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una publicación válida.")]
        public int IdPublicacion { get; set; }
        [JsonIgnore]
        public Publicacion Publicacion { get; set; }

        [Display(Name = "Id Habilidad")]//FK]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una habilidad válida.")]
        public int IdHabilidad { get; set; }
        [JsonIgnore]
        public Habilidad Habilidad { get; set; }
    }
}
