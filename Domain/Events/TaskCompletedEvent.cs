using MediatR;

namespace TodoApp.Domain.Events
{
    public record TaskCompletedEvent(int TaskId, string Title, int UserId) : INotification;
}
