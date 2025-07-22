using Microsoft.EntityFrameworkCore;
using Tactica.API.Configuration;
using Tactica.API.Data;
using Tactica.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with MySQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register all services
builder.Services.AddApplicationServices();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Temporary test route
app.MapGet("/healthcheck", () => Results.Ok("Tactica API is running"));
app.MapControllers();

app.UseHttpsRedirection();

app.Run();