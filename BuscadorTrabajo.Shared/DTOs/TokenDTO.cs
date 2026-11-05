namespace BuscadorTrabajo.Shared.DTOs
{
    // Respuesta del API al registrarse o iniciar sesión: el token JWT y cuándo vence.
    public class TokenDTO
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
