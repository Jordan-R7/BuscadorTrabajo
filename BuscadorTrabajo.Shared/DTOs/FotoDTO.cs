using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Foto de perfil que sube el usuario. La imagen viaja en Base64 (texto).
    // Solo se aceptan PNG, JPG y JPEG de máximo 2 MB.
    public class FotoDTO
    {
        [Required(ErrorMessage = "Debes elegir una imagen.")]
        public string Base64 { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreArchivo { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoContenido { get; set; }
    }
}
