namespace TodoApp.Application.Tests.Support;

public class ScenarioState
{
    public HttpClient Client { get; set; } = null!;
    public HttpResponseMessage Response { get; set; } = null!;
    public int CurrentUserId { get; set; }
    public int CurrentTaskId { get; set; }
    public string? AuthToken { get; set; }
    public string? CurrentUsername { get; set; }
    public List<HttpResponseMessage> Responses { get; set; } = new();
}
