using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BuscadorTrabajo.API.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(DataContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        // Crea el usuario; Identity guarda la contraseña cifrada (PasswordHash), nunca en texto plano
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        // Crea el rol si todavía no existe
        public async Task CheckRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
            }
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        // Revisa correo y contraseña. El "true" bloquea la cuenta unos minutos
        // después de varios intentos fallidos (se configura en Program.cs).
        // Se usa CheckPasswordSignInAsync porque el API no maneja cookies: la sesión es el token JWT.
        public async Task<SignInResult> LoginAsync(LoginDTO model)
        {
            var user = await GetUserAsync(model.Email);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        // Borra el usuario; sus roles (AspNetUserRoles) se borran con él
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<int> CountUsersInRoleAsync(string roleName)
        {
            return (await _userManager.GetUsersInRoleAsync(roleName)).Count;
        }

        // Identity revisa que la contraseña actual sea correcta antes de cambiarla
        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }
    }
}
