using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;
using TodoApp.Application.TSK001Tasks;
using TodoApp.Application.USR002Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TodoDb"));

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services
    .AddTasksFeature()
    .AddUsersFeature();

builder.Services.AddControllers();

// Note: In production, AllowAnyOrigin should be replaced with specific origins from configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("DefaultCorsPolicy");
app.MapControllers();
app.Run();

// Make Program class accessible for testing
public partial class Program { }
