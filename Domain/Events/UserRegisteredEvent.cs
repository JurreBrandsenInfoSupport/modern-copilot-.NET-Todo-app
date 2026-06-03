using MediatR;

namespace TodoApp.Domain.Events
{
    public record UserRegisteredEvent(int UserId, string Username) : INotification;
}
