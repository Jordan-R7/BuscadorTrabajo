using System.Globalization;
using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Helpers
{
    // Revisa las respuestas que llegan con una aplicación nueva (RN52 a RN56)
    // y las deja listas para guardarse junto con la aplicación.
    public static class ValidadorRespuestas
    {
        // Devuelve null si todo está bien, o el mensaje de error.
        public static async Task<string> ValidarAsync(DataContext context, Aplicacion aplicacion)
        {
            var preguntas = await context.Preguntas
                .AsNoTracking()
                .Where(x => x.IdPublicacion == aplicacion.IdPublicacion)
                .ToListAsync();

            // Las respuestas vacías se descartan (una pregunta opcional se puede dejar sin responder)
            var respuestas = (aplicacion.Respuestas ?? new List<Respuesta>())
                .Where(x => !string.IsNullOrWhiteSpace(x.Valor))
                .ToList();

            // RN54: una sola respuesta por pregunta en cada aplicación
            if (respuestas.GroupBy(x => x.IdPregunta).Any(g => g.Count() > 1))
            {
                return "Cada pregunta se responde una sola vez.";
            }

            foreach (var respuesta in respuestas)
            {
                // RN52 y RN53: la pregunta debe existir y ser de la publicación a la que se aplica
                var pregunta = preguntas.FirstOrDefault(x => x.Id == respuesta.IdPregunta);
                if (pregunta == null)
                {
                    return "Una de las respuestas es de una pregunta que no pertenece a esta publicación.";
                }

                // RN56: el valor debe corresponder al tipo de respuesta
                var valor = Normalizar(pregunta, respuesta.Valor.Trim());
                if (valor == null)
                {
                    return MensajeTipo(pregunta);
                }

                respuesta.Id = 0;
                respuesta.IdAplicacion = 0; // lo asigna la base de datos al guardar la aplicación
                respuesta.Valor = valor;
                respuesta.FechaRespuesta = DateTime.UtcNow;
            }

            // RN55: todas las preguntas obligatorias deben tener respuesta
            var sinResponder = preguntas
                .Where(p => p.Obligatoria && !respuestas.Any(r => r.IdPregunta == p.Id))
                .OrderBy(p => p.Orden)
                .FirstOrDefault();
            if (sinResponder != null)
            {
                return $"Falta responder la pregunta obligatoria: \"{sinResponder.Texto}\".";
            }

            aplicacion.Respuestas = respuestas;
            return null;
        }

        // Deja el valor en un formato único o devuelve null si no es válido para el tipo
        private static string Normalizar(Pregunta pregunta, string valor)
        {
            switch (pregunta.TipoRespuesta)
            {
                case "SI_NO":
                    var texto = valor.ToUpperInvariant().Replace("Í", "I");
                    return texto == "SI" || texto == "NO" ? texto : null;

                case "NUMERO":
                    return decimal.TryParse(valor.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var numero)
                        ? numero.ToString(CultureInfo.InvariantCulture)
                        : null;

                case "OPCION_UNICA":
                    return pregunta.ListaOpciones()
                        .FirstOrDefault(x => string.Equals(x, valor, StringComparison.OrdinalIgnoreCase));

                default: // TEXTO
                    return valor;
            }
        }

        private static string MensajeTipo(Pregunta pregunta)
        {
            return pregunta.TipoRespuesta switch
            {
                "SI_NO" => $"La pregunta \"{pregunta.Texto}\" se responde con SI o NO.",
                "NUMERO" => $"La pregunta \"{pregunta.Texto}\" se responde con un número.",
                "OPCION_UNICA" => $"La respuesta a \"{pregunta.Texto}\" debe ser una de las opciones: {string.Join(", ", pregunta.ListaOpciones())}.",
                _ => $"La respuesta a \"{pregunta.Texto}\" no es válida."
            };
        }
    }
}
