namespace TodoApp.Application.Tests.Support;

public class ScenarioState
{
    public HttpClient Client { get; set; } = null!;
    public HttpResponseMessage Response { get; set; } = null!;
    public int CurrentUserId { get; set; }
    public int CurrentTaskId { get; set; }
}
