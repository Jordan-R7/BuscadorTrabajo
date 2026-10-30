using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Perfil_Habilidad
    {
        public int Id { get; set; }

        [Display(Name = "Perfil Profesional")]//FK
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un perfil profesional válido.")]
        public int IdPerfilProfesional { get; set; }
        [JsonIgnore]
        public PerfilProfesional PerfilProfesional { get; set; }

        [Display(Name = "Habilidad")]//FK
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una habilidad válida.")]
        public int IdHabilidad { get; set; }
        [JsonIgnore]
        public Habilidad Habilidad { get; set; }
    }
}
