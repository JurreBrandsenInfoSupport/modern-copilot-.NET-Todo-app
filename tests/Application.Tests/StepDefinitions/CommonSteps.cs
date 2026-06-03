using System.Net;
using System.Text.Json;
using FluentAssertions;
using Reqnroll;
using TodoApp.Application.Tests.Support;

namespace TodoApp.Application.Tests.StepDefinitions;

[Binding]
public class CommonSteps
{
    private readonly ScenarioState _state;

    public CommonSteps(ScenarioState state)
    {
        _state = state;
    }

    [Given("the application is running")]
    public void GivenTheApplicationIsRunning()
    {
        _state.Client.Should().NotBeNull("the application should be running");
    }

    [Then("the response status code should be {int}")]
    public void ThenTheResponseStatusCodeShouldBe(int statusCode)
    {
        ((int)_state.Response.StatusCode).Should().Be(statusCode);
    }
}
