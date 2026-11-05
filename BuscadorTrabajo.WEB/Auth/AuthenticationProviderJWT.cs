using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using BuscadorTrabajo.WEB.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BuscadorTrabajo.WEB.Auth
{
    // Le dice a Blazor quién está conectado, leyendo el token JWT guardado en el navegador.
    // También pone el token en el encabezado Authorization de todas las peticiones al API.
    public class AuthenticationProviderJWT : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly HttpClient _httpClient;
        private readonly string _tokenKey;
        private readonly AuthenticationState _anonimous;

        public AuthenticationProviderJWT(IJSRuntime jSRuntime, HttpClient httpClient)
        {
            _jSRuntime = jSRuntime;
            _httpClient = httpClient;
            _tokenKey = "TOKEN_KEY";
            _anonimous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jSRuntime.GetLocalStorage(_tokenKey);
            if (token is null)
            {
                return _anonimous;
            }

            var claims = ParseClaimsFromJWT(token.ToString()!);
            if (claims == null)
            {
                // Token dañado o vencido: se cierra la sesión
                await _jSRuntime.RemoveLocalStorage(_tokenKey);
                _httpClient.DefaultRequestHeaders.Authorization = null;
                return _anonimous;
            }

            return BuildAuthenticationState(token.ToString()!, claims);
        }

        public async Task LoginAsync(string token)
        {
            await _jSRuntime.SetLocalStorage(_tokenKey, token);
            var authState = BuildAuthenticationState(token, ParseClaimsFromJWT(token) ?? new List<Claim>());
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task LogoutAsync()
        {
            await _jSRuntime.RemoveLocalStorage(_tokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonimous));
        }

        private AuthenticationState BuildAuthenticationState(string token, IEnumerable<Claim> claims)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        // Lee los datos (claims) del token. Devuelve null si el token no sirve o ya venció.
        private static List<Claim> ParseClaimsFromJWT(string token)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    return null;
                }

                // Dentro del token el nombre y el rol viajan como "unique_name" y "role";
                // se pasan a los tipos que Blazor usa para Identity.Name y para los roles.
                return jwt.Claims.Select(claim => claim.Type switch
                {
                    "unique_name" or "name" => new Claim(ClaimTypes.Name, claim.Value),
                    "role" => new Claim(ClaimTypes.Role, claim.Value),
                    _ => claim
                }).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
