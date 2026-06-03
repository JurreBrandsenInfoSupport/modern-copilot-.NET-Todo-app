using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Events;

namespace TodoApp.Application.EventHandlers
{
    public class CommentEventHandler : INotificationHandler<CommentAddedEvent>
    {
        private readonly ILogger<CommentEventHandler> _logger;

        public CommentEventHandler(ILogger<CommentEventHandler> logger) => _logger = logger;

        public Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Comment added: {CommentId} on Task {TaskItemId} by User {UserId} - '{Text}'",
                notification.CommentId, notification.TaskItemId, notification.UserId, notification.Text);
            return Task.CompletedTask;
        }
    }
}
