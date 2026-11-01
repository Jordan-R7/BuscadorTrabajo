using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.Shared.Validaciones
{
    // Igual que [Url], pero acepta el campo vacío.
    // [Url] rechaza el texto vacío (""), y eso pasaba en los formularios cuando el usuario
    // escribía un enlace y luego lo borraba. Con este atributo un enlace opcional puede
    // quedar vacío, y si se escribe algo, debe ser una dirección válida (http://, https://).
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UrlOpcionalAttribute : ValidationAttribute
    {
        private static readonly UrlAttribute validadorUrl = new();

        public override bool IsValid(object value)
        {
            if (value is null)
            {
                return true;
            }

            if (value is string texto && string.IsNullOrWhiteSpace(texto))
            {
                return true;
            }

            return validadorUrl.IsValid(value);
        }
    }
}
