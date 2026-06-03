using MediatR;

namespace TodoApp.Domain.Events
{
    public record CommentAddedEvent(int CommentId, int TaskItemId, int UserId, string Text) : INotification;
}
