using BuscadorTrabajo.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Conexión a SQL Server (la cadena DefaultConnection está en appsettings.json)
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

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
