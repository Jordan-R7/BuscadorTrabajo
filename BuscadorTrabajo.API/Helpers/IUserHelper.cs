using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.Shared.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BuscadorTrabajo.API.Helpers
{
    // Operaciones con usuarios y roles. Los controladores usan esta interfaz
    // en lugar de hablar directamente con UserManager, RoleManager y SignInManager.
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginDTO model);

        Task LogoutAsync();

        Task UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<int> CountUsersInRoleAsync(string roleName);
    }
}
