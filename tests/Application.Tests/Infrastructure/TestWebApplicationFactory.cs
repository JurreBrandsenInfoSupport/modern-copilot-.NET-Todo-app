using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TodoApp.Infrastructure;

namespace TodoApp.tests.Application.Tests.Infrastructure
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _databaseName;

        public TestWebApplicationFactory(string? databaseName = null)
        {
            _databaseName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Otel:Endpoint"] = "http://localhost:4317",
                    ["OTEL_SDK_DISABLED"] = "true",
                    ["Keycloak:TokenEndpoint"] = "http://fake-keycloak/token",
                    ["Keycloak:FrontendClientId"] = "todo-frontend"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
                services.RemoveAll(typeof(AppDbContext));

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                // Replace authentication with a test scheme that always succeeds
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                });

                // Replace the keycloak HTTP client with a fake that returns tokens
                services.AddHttpClient("keycloak")
                    .ConfigurePrimaryHttpMessageHandler(() => new FakeKeycloakHandler());

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });

            builder.UseEnvironment("Testing");
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // If no Authorization header, fail authentication (to test 401 scenarios)
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, "User"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    /// <summary>
    /// Fake HTTP handler that simulates Keycloak token endpoint responses for tests.
    /// </summary>
    public class FakeKeycloakHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = await request.Content!.ReadAsStringAsync(cancellationToken);
            var formValues = System.Web.HttpUtility.ParseQueryString(content);
            var username = formValues["username"];

            // Simulate Keycloak rejecting the specific "nonexistent" user
            if (username == "nonexistent")
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent(
                        """{"error":"invalid_grant","error_description":"Invalid user credentials"}""",
                        Encoding.UTF8, "application/json")
                };
            }

            // Generate a minimal valid JWT for testing
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("""{"alg":"HS256","typ":"JWT"}""")).TrimEnd('=');
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                $$"""{"sub":"1","preferred_username":"{{username}}","exp":9999999999}""")).TrimEnd('=');
            var fakeToken = $"{header}.{payload}.fakesignature";

            var responseJson = $$"""
            {
                "access_token": "{{fakeToken}}",
                "token_type": "Bearer",
                "expires_in": 3600
            }
            """;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        }
    }
}
