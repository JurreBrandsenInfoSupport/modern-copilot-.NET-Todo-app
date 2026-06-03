using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Events;

namespace TodoApp.Application.EventHandlers
{
    public class UserEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly ILogger<UserEventHandler> _logger;

        public UserEventHandler(ILogger<UserEventHandler> logger) => _logger = logger;

        public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User registered: {UserId} - '{Username}'",
                notification.UserId, notification.Username);
            return Task.CompletedTask;
        }
    }
}
