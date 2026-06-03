using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginRequest request)
        {
            var tokenEndpoint = _configuration["Keycloak:TokenEndpoint"]
                ?? "http://localhost:8080/realms/todoapp/protocol/openid-connect/token";
            var clientId = _configuration["Keycloak:FrontendClientId"] ?? "todo-frontend";

            var client = _httpClientFactory.CreateClient("keycloak");

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = clientId,
                ["username"] = request.Username,
                ["password"] = request.Password ?? "demo",
                ["scope"] = "openid profile"
            };

            var response = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized(new { error = "Invalid credentials" });
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);

            var accessToken = tokenResponse.GetProperty("access_token").GetString();
            var expiresIn = tokenResponse.GetProperty("expires_in").GetInt32();
            var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

            return Ok(new { token = accessToken, expiresAt });
        }
    }

    public record LoginRequest(string Username, string? Password = null);
}
