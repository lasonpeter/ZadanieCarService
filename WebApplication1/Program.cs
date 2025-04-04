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

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: "logs/log_.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

//Can be also made to always log to console
if (builder.Environment.IsDevelopment())
{
    loggerConfiguration.WriteTo.Console();
}

Log.Logger = loggerConfiguration.CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    if (builder.Environment.IsDevelopment())
        options.UseInMemoryDatabase("UserDb");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddTransient<IValidator<User>, UserValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();