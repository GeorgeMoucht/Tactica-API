using Microsoft.EntityFrameworkCore;
using Tactica.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with MySQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Temporary test route
app.MapGet("/healthcheck", () => Results.Ok("Tactica API is running"));

app.UseHttpsRedirection();

app.Run();