using MediatR;

namespace TodoApp.Domain.Events
{
    public record TaskCreatedEvent(int TaskId, string Title, int UserId) : INotification;
}
