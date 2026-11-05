using System.Text;
using BuscadorTrabajo.API.Data;
using BuscadorTrabajo.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Swagger con el botón "Authorize" para probar los endpoints protegidos con el token JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BuscadorTrabajo API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Autorización JWT con el esquema Bearer. <br /> <br />
                      Inicia sesión en POST /api/accounts/Login, copia el token y pégalo aquí.<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Conexión a SQL Server (la cadena DefaultConnection está en appsettings.json)
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=DefaultConnection"));

// Alimentador de la base de datos con datos iniciales
builder.Services.AddTransient<SeedDb>();

// Usuarios y roles con ASP.NET Identity
builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Password.RequiredLength = 6;
    // Después de 5 intentos fallidos la cuenta se bloquea 5 minutos
    x.Lockout.MaxFailedAccessAttempts = 5;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    x.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserHelper, UserHelper>();

// Autenticación con token JWT. La llave (jwtKey) está en appsettings.json;
// en Azure se reemplaza desde la configuración del App Service.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
        ClockSkew = TimeSpan.Zero
    });

var app = builder.Build();

SeedData(app);

// Swagger queda activo también en Azure para poder probar el API desplegado
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BuscadorTrabajo API v1");
});

app.UseHttpsRedirection();

// Permite que la aplicación Blazor (otro puerto) consuma el API
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

// Primero se identifica quién es (token) y luego se revisa qué puede hacer (roles)
app.UseAuthentication();
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
