using BuscadorTrabajo.WEB;
using BuscadorTrabajo.WEB.Auth;
using BuscadorTrabajo.WEB.Repositories;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Dirección del API: se lee de wwwroot/appsettings.json ("ApiUrl").
// En local es https://localhost:7000/; al publicar en Azure se cambia por la dirección del API en Azure.
var apiUrl = builder.Configuration["ApiUrl"] ?? "https://localhost:7000/";
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

// Repositorio que centraliza las llamadas HTTP al API
builder.Services.AddScoped<IRepository, Repository>();

// Alertas bonitas (confirmaciones y mensajes de error)
builder.Services.AddSweetAlert2();

// Seguridad: el mismo AuthenticationProviderJWT dice quién está conectado
// (AuthenticationStateProvider) y abre o cierra la sesión (ILoginService)
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());

await builder.Build().RunAsync();
