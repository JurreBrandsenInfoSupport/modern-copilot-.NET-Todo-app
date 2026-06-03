using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Events;

namespace TodoApp.Application.EventHandlers
{
    public class TaskEventHandler :
        INotificationHandler<TaskCreatedEvent>,
        INotificationHandler<TaskCompletedEvent>
    {
        private readonly ILogger<TaskEventHandler> _logger;

        public TaskEventHandler(ILogger<TaskEventHandler> logger) => _logger = logger;

        public Task Handle(TaskCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task created: {TaskId} - '{Title}' by User {UserId}",
                notification.TaskId, notification.Title, notification.UserId);
            return Task.CompletedTask;
        }

        public Task Handle(TaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task completed: {TaskId} - '{Title}' by User {UserId}",
                notification.TaskId, notification.Title, notification.UserId);
            return Task.CompletedTask;
        }
    }
}
