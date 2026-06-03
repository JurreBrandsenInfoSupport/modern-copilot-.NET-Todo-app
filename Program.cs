using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running."));

var app = builder.Build();
app.UseCors("DefaultCorsPolicy");
app.MapControllers();

var healthCheckOptions = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        });
        await context.Response.WriteAsync(result);
    }
};

app.MapHealthChecks("/health", healthCheckOptions);
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = healthCheckOptions.ResponseWriter
});

app.Run();

// Make Program class accessible for testing
public partial class Program { }
