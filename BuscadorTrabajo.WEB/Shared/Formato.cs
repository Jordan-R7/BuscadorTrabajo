using System.Globalization;

namespace BuscadorTrabajo.WEB.Shared
{
    // Funciones pequeñas para mostrar datos de forma amigable en las páginas.
    public static class Formato
    {
        // 3500000 y "COP" -> "$3.500.000 COP"
        public static string Dinero(decimal? valor, string moneda)
        {
            if (valor == null)
            {
                return "Sin definir";
            }

            var texto = valor.Value.ToString("#,0", CultureInfo.InvariantCulture).Replace(",", ".");
            return $"${texto} {moneda}";
        }

        // 2026-12-31 -> "31/12/2026"
        public static string Fecha(DateOnly? fecha)
        {
            return fecha == null ? "Sin definir" : fecha.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        // "TecnoAndes S.A.S." -> "TS"
        public static string Iniciales(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return "?";
            }

            var palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var iniciales = palabras.Take(2).Select(p => char.ToUpperInvariant(p[0]));
            return string.Concat(iniciales);
        }

        // "PUBLICADA" -> "Publicada", "HIBRIDO" -> "Híbrido"
        public static string Texto(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return "Sin definir";
            }

            return valor switch
            {
                "EMPLEO" => "Empleo",
                "SERVICIO" => "Servicio",
                "BORRADOR" => "Borrador",
                "PUBLICADA" => "Publicada",
                "PAUSADA" => "Pausada",
                "CERRADA" => "Cerrada",
                "CANCELADA" => "Cancelada",
                "REMOTO" => "Remoto",
                "HIBRIDO" => "Híbrido",
                "PRESENCIAL" => "Presencial",
                "PENDIENTE" => "Pendiente",
                "EN_REVISION" => "En revisión",
                "ACEPTADA" => "Aceptada",
                "RECHAZADA" => "Rechazada",
                "RETIRADA" => "Retirada",
                _ => valor
            };
        }
    }
}
