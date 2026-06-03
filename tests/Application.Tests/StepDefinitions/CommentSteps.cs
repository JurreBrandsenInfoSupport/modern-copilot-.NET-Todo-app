using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class CommentSteps
{
    private readonly ScenarioState _state;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CommentSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("a comment {string} exists on the task")]
    public async Task GivenACommentExistsOnTheTask(string text)
    {
        var response = await _state.Client.PostAsJsonAsync("/api/v1/comments",
            new { TaskItemId = _state.CurrentTaskId, UserId = _state.CurrentUserId, Text = text });
        response.EnsureSuccessStatusCode();
    }

    [When("I add a comment {string} to the task")]
    public async Task WhenIAddACommentToTheTask(string text)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/v1/comments",
            new { TaskItemId = _state.CurrentTaskId, UserId = _state.CurrentUserId, Text = text });
    }

    [When("I add a comment {string} to task id {int}")]
    public async Task WhenIAddACommentToTaskId(string text, int taskId)
    {
        _state.Response = await _state.Client.PostAsJsonAsync("/api/v1/comments",
            new { TaskItemId = taskId, UserId = _state.CurrentUserId, Text = text });
    }

    [When("I request comments for the task")]
    public async Task WhenIRequestCommentsForTheTask()
    {
        _state.Response = await _state.Client.GetAsync($"/api/v1/comments?taskItemId={_state.CurrentTaskId}");
    }

    [Then("the response should contain a comment with text {string}")]
    public async Task ThenTheResponseShouldContainACommentWithText(string text)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var comment = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        comment.GetProperty("text").GetString().Should().Be(text);
    }

    [Then("the response should contain {int} comments")]
    public async Task ThenTheResponseShouldContainNComments(int count)
    {
        var content = await _state.Response.Content.ReadAsStringAsync();
        var comments = JsonSerializer.Deserialize<JsonElement>(content, JsonOptions);
        comments.GetArrayLength().Should().Be(count);
    }
}
