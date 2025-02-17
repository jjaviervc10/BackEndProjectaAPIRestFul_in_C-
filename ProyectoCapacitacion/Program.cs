using Microsoft.EntityFrameworkCore;
using ProyectoCapacitacion.Data;
//using ProyectoCapacitacion.Data.Repositories;
//using ProyectoCapacitacion.Repositories;
//using ProyectoCapacitacion.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar el DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // Cambia "DefaultConnection" a tu clave de cadena de conexi�n en appsettings.json

// Configuraci�n de servicios y repositorios
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
/*builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IEmpresaService, EmpresaServices>();

builder.Services.AddScoped<ISucursalRepository, SucursalRepository>();
builder.Services.AddScoped<ISucursalService, SucursalService>();*/

// A�adir servicios a la colecci�n de dependencias
builder.Services.AddControllers();
builder.Services.AddLogging();

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Permite solicitudes desde cualquier origen
              .AllowAnyHeader()  // Permite cualquier encabezado
              .AllowAnyMethod();  // Permite cualquier m�todo HTTP (GET, POST, PUT, DELETE)
    });
});

// Configuraci�n de Swagger para documentaci�n y pruebas de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar CORS
app.UseCors("AllowAll");  // Aplica la pol�tica de CORS llamada "AllowAll"

// Solo habilitar redirecci�n HTTPS en producci�n
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();  // Solo redirigir a HTTPS en entornos que no son de desarrollo
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita Swagger en el entorno de desarrollo
    app.UseSwaggerUI();  // Interfaz de usuario de Swagger
}

app.UseAuthorization();  // Habilita autorizaci�n para proteger la API

app.MapControllers();  // Mapea los controladores definidos en el proyecto

app.Run();  // Inicia la aplicaci�n
