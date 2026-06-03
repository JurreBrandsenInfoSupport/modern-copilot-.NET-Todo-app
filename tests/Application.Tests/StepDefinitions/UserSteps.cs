using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class UserSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public UserSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("a user {string} exists")]
    public async Task GivenAUserExists(string username)
    {
        var response = await _state.Client.PostAsJsonAsync("/api/v1/users", new { Username = username });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        _state.CurrentUserId = user.GetProperty("id").GetInt32();
    }

    [When("I register a user with username {string}")]
    public async Task WhenIRegisterAUserWithUsername(string username)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/v1/users", new { Username = username });
    }

    [When("I request all users")]
    public async Task WhenIRequestAllUsers()
    {
        _state.Response = await _state.Client.GetAsync("/api/v1/users");
    }

    [Then("the response should contain a user with username {string}")]
    public async Task ThenTheResponseShouldContainAUserWithUsername(string username)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        user.GetProperty("username").GetString().Should().Be(username);
    }

    [Then("the response should contain {int} users")]
    public async Task ThenTheResponseShouldContainNUsers(int count)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        users.GetArrayLength().Should().Be(count);
    }
}
