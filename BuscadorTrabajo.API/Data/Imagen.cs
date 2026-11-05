using System.ComponentModel.DataAnnotations;

namespace BuscadorTrabajo.API.Data
{
    // Imagen guardada en la base de datos (por ahora, las fotos de perfil).
    // Cada imagen tiene su propio Id; el usuario guarda ese Id en FotoId.
    // Así, en SQL Server se ve quién tiene foto (FotoId con valor) y quién no (FotoId en NULL).
    public class Imagen
    {
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string NombreArchivo { get; set; }

        // image/png o image/jpeg (solo PNG, JPG y JPEG)
        [Required]
        [MaxLength(50)]
        public string TipoContenido { get; set; }

        [Required]
        public byte[] Datos { get; set; }

        // Tamaño en bytes
        public int Tamano { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    }
}
