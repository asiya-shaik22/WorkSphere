using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add OpenAPI
builder.Services.AddOpenApi();

// Database connection
string? ConStr = builder.Configuration
    .GetSection("ConnectionStrings")["MyConn"];

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(ConStr));

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();