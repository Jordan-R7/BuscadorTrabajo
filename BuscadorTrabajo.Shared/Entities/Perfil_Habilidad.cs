using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace BuscadorTrabajo.Shared.Entities
{
    public class Perfil_Habilidad
    {
        [Display(Name = "Perfil Profesional")]//FK
        public int IdPerfilProfesional { get; set; }
        [JsonIgnore]
        public PerfilProfesional PerfilProfesional { get; set; }

        [Display(Name = "Habilidad")]//FK
        public int IdHabilidad { get; set; }
        [JsonIgnore]
        public Habilidad Habilidad { get; set; }
    }
}
