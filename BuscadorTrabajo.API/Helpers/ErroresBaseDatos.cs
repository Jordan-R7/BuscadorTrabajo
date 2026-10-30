using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Helpers
{
    // Identifica qué regla de la base de datos se incumplió al guardar,
    // para responder con un mensaje claro en lugar de un error 500.
    public static class ErroresBaseDatos
    {
        // 2601 y 2627: se repitió un valor que debe ser único (cédula, NIT, nombre...)
        public static bool EsDuplicado(DbUpdateException ex) =>
            ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);

        // 547: se violó una llave foránea (el registro está en uso o la relación no existe)
        public static bool EsRelacion(DbUpdateException ex) =>
            ex.InnerException is SqlException sql && sql.Number == 547;
    }
}
