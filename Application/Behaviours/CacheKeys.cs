namespace TodoApp.Application.Behaviours;

/// <summary>
/// Centralised cache key constants to avoid magic strings.
/// </summary>
public static class CacheKeys
{
    public const string AllTasks = "tasks:all";
    public const string AllUsers = "users:all";

    public static string TasksByUser(int userId) => $"tasks:user:{userId}";
    public static string CommentsByTask(int taskItemId) => $"comments:task:{taskItemId}";
}
