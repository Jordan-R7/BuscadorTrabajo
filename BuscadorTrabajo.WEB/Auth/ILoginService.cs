namespace BuscadorTrabajo.WEB.Auth
{
    // Lo usan las páginas de Login, Registro y Logout para abrir o cerrar la sesión.
    public interface ILoginService
    {
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}
