using BuscadorTrabajo.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Conexión a SQL Server (la cadena DefaultConnection está en appsettings.json)
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=DefaultConnection"));

// Alimentador de la base de datos con datos iniciales
builder.Services.AddTransient<SeedDb>();

var app = builder.Build();

SeedData(app);

if (app.Environment.IsDevelopment())

{

    app.MapOpenApi();

    app.UseSwaggerUI(options =>

    {

        options.SwaggerEndpoint("/openapi/v1.json", "v1");

    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Ejecuta el SeedDb al iniciar la aplicación
void SeedData(WebApplication app)
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (IServiceScope scope = scopedFactory!.CreateScope())
    {
        SeedDb service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedDbAsync().Wait();
    }
}
