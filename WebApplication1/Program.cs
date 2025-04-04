using FluentValidation;
using Serilog;
using Serilog.Events;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Validators;

var builder = WebApplication.CreateBuilder(args);

// Ensure logs directory exists
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "logs"));

// Clear any existing log files
if (Directory.Exists("logs"))
{
    foreach (var file in Directory.GetFiles("logs", "*.txt"))
    {
        try { File.Delete(file); } catch { }
    }
}

// Configure Serilog
var logConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration);

Log.Logger = logConfig.CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => 
    {
        fv.DisableDataAnnotationsValidation = true;
        fv.ImplicitlyValidateChildProperties = false;
        fv.AutomaticValidationEnabled = false;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}