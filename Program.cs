using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi; // Soporte nativo de OpenAPI en .NET 10

var builder = WebApplication.CreateBuilder(args);

// ===== DbContext =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Controllers =====
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ===== OpenAPI / Swagger =====
builder.Services.AddEndpointsApiExplorer(); // Para generar el JSON de OpenAPI
builder.Services.AddSwaggerGen();           // Configuración básica, no necesita OpenApiInfo

// Para evitar problemas con fechas infinitas de Npgsql
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var app = builder.Build();

// ===== Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // Genera /swagger/v1/swagger.json
    app.UseSwaggerUI();      // Interfaz en /swagger
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
