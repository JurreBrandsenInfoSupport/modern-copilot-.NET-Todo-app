using Reqnroll;
using TodoApp.Application.Tests.Support;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.Application.Tests.Hooks;

[Binding]
public class TestHooks
{
    private TestWebApplicationFactory<Program>? _factory;

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        _factory = new TestWebApplicationFactory<Program>();
        var client = _factory.CreateClient();

        var state = scenarioContext.ScenarioContainer.Resolve<ScenarioState>();
        state.Client = client;
    }

    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext)
    {
        var state = scenarioContext.ScenarioContainer.Resolve<ScenarioState>();
        state.Client?.Dispose();
        _factory?.Dispose();
    }
}
