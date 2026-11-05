using System.ComponentModel.DataAnnotations;
using BuscadorTrabajo.Shared.Enums;
using BuscadorTrabajo.Shared.Validaciones;

namespace BuscadorTrabajo.Shared.DTOs
{
    // Registro de una cuenta de Empresa, Cliente o Administrador.
    // Crea el usuario (correo + contraseña + rol) y su registro en la tabla del rol,
    // vinculados por el Id del usuario. Los desarrolladores se registran con el
    // asistente de varios pasos (RegistroDesarrolladorDTO).
    public class RegistroUsuarioDTO : IValidatableObject
    {
        [Display(Name = "Tipo de cuenta")]
        public UserType TipoUsuario { get; set; } = UserType.Cliente;

        // ---------- Datos de acceso ----------
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido")]
        [MaxLength(300, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirmación de contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Compare(nameof(Password), ErrorMessage = "La contraseña y la confirmación no son iguales")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Celular")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(20, ErrorMessage = "El {0} no puede tener más de {1} caracteres")]
        [Phone(ErrorMessage = "El celular debe tener un formato válido")]
        public string Celular { get; set; }

        // ---------- Persona (Cliente y Administrador) ----------
        [Display(Name = "Cédula")]
        [MaxLength(10, ErrorMessage = "La Cédula no puede tener más de 10 caracteres")]
        public string Cedula { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(100, ErrorMessage = "El Nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        [MaxLength(100, ErrorMessage = "El Apellido no puede tener más de 100 caracteres")]
        public string Apellido { get; set; }

        // ---------- Empresa ----------
        [Display(Name = "Nombre de la empresa")]
        [MaxLength(50, ErrorMessage = "El nombre de la empresa no puede tener más de 50 caracteres")]
        public string NombreEmpresa { get; set; }

        [Display(Name = "NIT")]
        [MaxLength(12, ErrorMessage = "El NIT no puede tener más de 12 caracteres")]
        public string NIT { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "País")]
        [MaxLength(50, ErrorMessage = "El país no puede tener más de 50 caracteres")]
        public string Pais { get; set; }

        [Display(Name = "Ciudad")]
        [MaxLength(50, ErrorMessage = "La ciudad no puede tener más de 50 caracteres")]
        public string Ciudad { get; set; }

        [Display(Name = "Dirección")]
        [MaxLength(150, ErrorMessage = "La dirección no puede tener más de 150 caracteres")]
        public string Direccion { get; set; }

        [Display(Name = "Sitio web")]
        [MaxLength(300, ErrorMessage = "El sitio web no puede tener más de 300 caracteres")]
        [UrlOpcional(ErrorMessage = "El sitio web debe ser una URL válida")]
        public string SitioWeb { get; set; }

        // Los campos obligatorios cambian según el tipo de cuenta.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoUsuario == UserType.Desarrollador)
            {
                yield return new ValidationResult(
                    "Los desarrolladores se registran con el asistente de registro de desarrollador.",
                    new[] { nameof(TipoUsuario) });
                yield break;
            }

            if (TipoUsuario == UserType.Empresa)
            {
                foreach (var error in Obligatorios(
                    (NombreEmpresa, nameof(NombreEmpresa), "El nombre de la empresa es obligatorio."),
                    (NIT, nameof(NIT), "El NIT es obligatorio."),
                    (Descripcion, nameof(Descripcion), "La descripción es obligatoria."),
                    (Pais, nameof(Pais), "El país es obligatorio."),
                    (Ciudad, nameof(Ciudad), "La ciudad es obligatoria."),
                    (Direccion, nameof(Direccion), "La dirección es obligatoria.")))
                {
                    yield return error;
                }
                yield break;
            }

            // Cliente y Administrador son personas
            foreach (var error in Obligatorios(
                (Cedula, nameof(Cedula), "La cédula es obligatoria."),
                (Nombre, nameof(Nombre), "El nombre es obligatorio."),
                (Apellido, nameof(Apellido), "El apellido es obligatorio.")))
            {
                yield return error;
            }

            if (!string.IsNullOrWhiteSpace(Cedula)
                && (Cedula.Length < 6 || !Cedula.All(char.IsDigit)))
            {
                yield return new ValidationResult(
                    "La cédula solo debe contener números y tener entre 6 y 10 dígitos.",
                    new[] { nameof(Cedula) });
            }
        }

        private static IEnumerable<ValidationResult> Obligatorios(params (string valor, string campo, string mensaje)[] campos)
        {
            foreach (var (valor, campo, mensaje) in campos)
            {
                if (string.IsNullOrWhiteSpace(valor))
                {
                    yield return new ValidationResult(mensaje, new[] { campo });
                }
            }
        }
    }
}
