using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Datos de la cuenta del usuario conectado (página "Mi cuenta").
    // El correo y el tipo de cuenta solo se muestran: no se cambian aquí.
    public class CuentaDTO
    {
        public string Email { get; set; }

        public string TipoUsuario { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(100, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        public string Nombre { get; set; }

        // Las empresas no tienen apellido
        [Display(Name = "Apellido")]
        [MaxLength(100, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        public string Apellido { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        [Phone(ErrorMessage = "El celular debe tener un formato válido")]
        public string Celular { get; set; }

        // Id de la foto de perfil (null = sin foto). Se ve en /api/imagenes/{FotoId}
        public Guid? FotoId { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime? UltimoAcceso { get; set; }
    }
}
