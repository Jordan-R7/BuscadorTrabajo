using System.Net;
using System.Text.Json;

namespace BuscadorTrabajo.WEB.Repositories
{
    // Envuelve la respuesta del API: el dato recibido, si hubo error y la respuesta HTTP completa.
    public class HttpResponseWrapper<T>
    {
        public HttpResponseWrapper(T response, bool error, HttpResponseMessage httpResponseMessage)
        {
            Error = error;
            Response = response;
            HttpResponseMessage = httpResponseMessage;
        }

        public bool Error { get; set; }

        public T Response { get; set; }

        public HttpResponseMessage HttpResponseMessage { get; set; }

        // Convierte el código de error HTTP en un mensaje entendible para el usuario.
        public async Task<string> GetErrorMessageAsync()
        {
            if (!Error)
            {
                return null;
            }

            var codigoEstatus = HttpResponseMessage.StatusCode;
            var contenido = await HttpResponseMessage.Content.ReadAsStringAsync();

            if (codigoEstatus == HttpStatusCode.NotFound)
            {
                return ExtraerMensaje(contenido, "Recurso no encontrado.");
            }
            else if (codigoEstatus == HttpStatusCode.BadRequest)
            {
                return ExtraerMensaje(contenido, "Los datos enviados no son válidos.");
            }
            else if (codigoEstatus == HttpStatusCode.Conflict)
            {
                return ExtraerMensaje(contenido, "La operación entra en conflicto con otro registro.");
            }
            else if (codigoEstatus == HttpStatusCode.ServiceUnavailable)
            {
                return ExtraerMensaje(contenido, "No se pudo conectar con el servidor.");
            }
            else if (codigoEstatus == HttpStatusCode.Unauthorized)
            {
                return "Debes iniciar sesión para realizar esta acción.";
            }
            else if (codigoEstatus == HttpStatusCode.Forbidden)
            {
                return "No tienes permisos para realizar esta acción.";
            }

            return "Ha ocurrido un error inesperado en el servidor. Inténtalo de nuevo.";
        }

        // Algunas respuestas del API traen un código para que la página sepa qué hacer,
        // por ejemplo { "codigo": "REQUIERE_CONFIRMACION_HISTORIAL", "mensaje": "..." }.
        // Devuelve ese código, o null si la respuesta no lo trae.
        public async Task<string> GetCodigoErrorAsync()
        {
            if (!Error)
            {
                return null;
            }

            var contenido = await HttpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(contenido) || !contenido.TrimStart().StartsWith("{"))
            {
                return null;
            }

            try
            {
                using var documento = JsonDocument.Parse(contenido);
                if (documento.RootElement.TryGetProperty("codigo", out var codigo)
                    && codigo.ValueKind == JsonValueKind.String)
                {
                    return codigo.GetString();
                }
            }
            catch (JsonException)
            {
            }

            return null;
        }

        // El API responde un texto simple (por ejemplo "Ya existe una categoría con ese nombre.")
        // o un JSON con la lista de errores de validación. Aquí se obtiene un mensaje legible de ambos.
        private static string ExtraerMensaje(string contenido, string mensajePorDefecto)
        {
            if (string.IsNullOrWhiteSpace(contenido))
            {
                return mensajePorDefecto;
            }

            contenido = contenido.Trim();
            if (!contenido.StartsWith("{"))
            {
                return contenido.Trim('"');
            }

            try
            {
                using var documento = JsonDocument.Parse(contenido);

                // Respuesta con código y mensaje: { "codigo": "...", "mensaje": "..." }
                if (documento.RootElement.TryGetProperty("mensaje", out var mensajeApi)
                    && mensajeApi.ValueKind == JsonValueKind.String)
                {
                    return mensajeApi.GetString();
                }

                if (documento.RootElement.TryGetProperty("errors", out var errores)
                    && errores.ValueKind == JsonValueKind.Object)
                {
                    var mensajes = new List<string>();
                    foreach (var campo in errores.EnumerateObject())
                    {
                        foreach (var mensaje in campo.Value.EnumerateArray())
                        {
                            mensajes.Add(mensaje.GetString());
                        }
                    }

                    if (mensajes.Count > 0)
                    {
                        return string.Join(" ", mensajes);
                    }
                }
            }
            catch (JsonException)
            {
            }

            return mensajePorDefecto;
        }
    }
}
