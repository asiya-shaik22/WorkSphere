using Microsoft.EntityFrameworkCore;
using WorkSphere.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();