using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TodoApp.Infrastructure;
using TodoApp.Application.TSK001Tasks;
using TodoApp.Application.USR002Users;
using TodoApp.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (builder.Environment.IsEnvironment("Testing") || string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("TodoDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseNpgsql(connectionString));
}

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services
    .AddTasksFeature()
    .AddUsersFeature();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-Api-Version"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsADemoSecretKeyThatShouldBeChanged123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TodoApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TodoApp";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TodoApp API",
        Version = "v1",
        Description = "Enterprise-grade Todo application API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(opts => opts.Endpoint = new Uri(builder.Configuration["Otel:Endpoint"] ?? "http://localhost:4317")))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opts => opts.Endpoint = new Uri(builder.Configuration["Otel:Endpoint"] ?? "http://localhost:4317")));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("DefaultCorsPolicy");
app.UseSerilogRequestLogging();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
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

// Apply migrations and seed demo user on startup (skip in test environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.IsRelational())
    {
        await db.Database.MigrateAsync();
    }
    else
    {
        db.Database.EnsureCreated();
    }

    if (!db.Users.Any(u => u.Username == "demo"))
    {
        db.Users.Add(new TodoApp.Domain.Entities.User { Username = "demo" });
        await db.SaveChangesAsync();
    }
}

await app.RunAsync();

// Make Program class accessible for testing
public partial class Program { }
