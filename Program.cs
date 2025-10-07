using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure;
using TodoApp.Application.TSK001Tasks;
using TodoApp.Application.USR002Users;
using TodoApp.Application.BAL004BalloonAnimals;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TodoDb"));

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services
    .AddTasksFeature()
    .AddUsersFeature()
    .AddBalloonAnimalsFeature();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();

// Make Program class accessible for testing
public partial class Program { }
