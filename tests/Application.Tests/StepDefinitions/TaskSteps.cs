using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class TaskSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TaskSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("a task {string} exists for the user")]
    public async Task GivenATaskExistsForTheUser(string title)
    {
        var response = await _state.Client.PostAsJsonAsync("/api/v1/tasks",
            new { Title = title, UserId = _state.CurrentUserId });
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var task = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        _state.CurrentTaskId = task.GetProperty("id").GetInt32();
    }

    [When("I create a task with title {string} for the user")]
    public async Task WhenICreateATaskWithTitleForTheUser(string title)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/v1/tasks",
            new { Title = title, UserId = _state.CurrentUserId });
    }

    [When("I create a task with title {string} for user id {int}")]
    public async Task WhenICreateATaskWithTitleForUserId(string title, int userId)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/v1/tasks",
            new { Title = title, UserId = userId });
    }

    [When("I request all tasks")]
    public async Task WhenIRequestAllTasks()
    {
        _state.Response = await _state.Client.GetAsync("/api/v1/tasks");
    }

    [When("I request tasks for the user")]
    public async Task WhenIRequestTasksForTheUser()
    {
        _state.Response = await _state.Client.GetAsync($"/api/v1/tasks?userId={_state.CurrentUserId}");
    }

    [Then("the response should contain a task with title {string}")]
    public async Task ThenTheResponseShouldContainATaskWithTitle(string title)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);

        if (json.ValueKind == JsonValueKind.Array)
        {
            json.EnumerateArray().Should().Contain(t => t.GetProperty("title").GetString() == title);
        }
        else
        {
            json.GetProperty("title").GetString().Should().Be(title);
        }
    }

    [Then("the response should contain {int} tasks")]
    public async Task ThenTheResponseShouldContainNTasks(int count)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        tasks.GetArrayLength().Should().Be(count);
    }
}
