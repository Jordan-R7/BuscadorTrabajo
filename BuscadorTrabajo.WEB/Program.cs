using BuscadorTrabajo.WEB;
using BuscadorTrabajo.WEB.Repositories;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Dirección del API (perfil https de BuscadorTrabajo.API). Si cambia el puerto del API, cámbialo aquí.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7000/") });

// Repositorio que centraliza las llamadas HTTP al API
builder.Services.AddScoped<IRepository, Repository>();

// Alertas bonitas (confirmaciones y mensajes de error)
builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();
