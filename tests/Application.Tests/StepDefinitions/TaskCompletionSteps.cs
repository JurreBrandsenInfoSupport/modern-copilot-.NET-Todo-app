using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class TaskCompletionSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TaskCompletionSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("the task is already completed")]
    public async Task GivenTheTaskIsAlreadyCompleted()
    {
        var response = await _state.Client.PutAsync($"/api/v1/tasks/{_state.CurrentTaskId}/complete", null);
        response.EnsureSuccessStatusCode();
    }

    [When("I complete the task")]
    public async Task WhenICompleteTheTask()
    {
        _state.Response = await _state.Client.PutAsync($"/api/v1/tasks/{_state.CurrentTaskId}/complete", null);
    }

    [When("I complete task with id {int}")]
    public async Task WhenICompleteTaskWithId(int taskId)
    {
        _state.Response = await _state.Client.PutAsync($"/api/v1/tasks/{taskId}/complete", null);
    }

    [Then("the task should be marked as completed")]
    public async Task ThenTheTaskShouldBeMarkedAsCompleted()
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        task.GetProperty("isCompleted").GetBoolean().Should().BeTrue();
    }
}
