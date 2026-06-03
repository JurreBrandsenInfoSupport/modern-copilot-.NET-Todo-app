using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class HealthCheckSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public HealthCheckSteps(ScenarioState state)
    {
        _state = state;
    }

    [When("I request the health endpoint")]
    public async Task WhenIRequestTheHealthEndpoint()
    {
        _state.Response = await _state.Client.GetAsync("/health");
    }

    [When("I request the readiness endpoint")]
    public async Task WhenIRequestTheReadinessEndpoint()
    {
        _state.Response = await _state.Client.GetAsync("/health/ready");
    }

    [Then("the health status should be {string}")]
    public async Task ThenTheHealthStatusShouldBe(string expectedStatus)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        json.GetProperty("status").GetString().Should().Be(expectedStatus);
    }

    [Then("the response should contain health check entries")]
    public async Task ThenTheResponseShouldContainHealthCheckEntries()
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        var checks = json.GetProperty("checks");
        checks.GetArrayLength().Should().BeGreaterThan(0);
    }
}
