using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class AuthSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("I am authenticated as {string}")]
    public async Task GivenIAmAuthenticatedAs(string username)
    {
        var response = await _state.Client.PostAsJsonAsync("/api/auth/token", new { Username = username });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        _state.AuthToken = json.GetProperty("token").GetString();
        _state.CurrentUsername = username;

        _state.Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _state.AuthToken);
    }

    [When("I request a token for username {string}")]
    public async Task WhenIRequestATokenForUsername(string username)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/auth/token", new { Username = username });
    }

    [When("I request all tasks without authentication")]
    public async Task WhenIRequestAllTasksWithoutAuthentication()
    {
        _state.Client.DefaultRequestHeaders.Authorization = null;
        _state.Response = await _state.Client.GetAsync("/api/v1/tasks");
    }

    [Then("the response should contain a valid JWT token")]
    public async Task ThenTheResponseShouldContainAValidJwtToken()
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        var token = json.GetProperty("token").GetString();

        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
    }

    [Then("the token should expire in the future")]
    public async Task ThenTheTokenShouldExpireInTheFuture()
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        var expiresAt = json.GetProperty("expiresAt").GetDateTime();

        expiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}
