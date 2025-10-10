using MediatR;
using TodoApp.Application.RMD001Reminders;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.RMD001Reminders
{
    public class ReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReminderBackgroundService> _logger;

        public ReminderBackgroundService(IServiceProvider serviceProvider, ILogger<ReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reminder Background Service started");
            
            using PeriodicTimer timer = new(TimeSpan.FromMinutes(15));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ProcessPendingReminders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing reminders");
                }
            }
        }

        private async Task ProcessPendingReminders(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var pendingReminders = await mediator.Send(new GetPendingRemindersQuery(), cancellationToken);

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    // For now, we'll just log the reminder. In the future, this could send emails, push notifications, etc.
                    await SendReminderNotification(reminder);

                    // Mark the reminder as sent
                    await mediator.Send(new MarkReminderSentCommand(reminder.Id), cancellationToken);

                    _logger.LogInformation("Reminder sent for Task ID {TaskId}, Type: {ReminderType}", 
                        reminder.TaskItemId, reminder.Type);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process reminder {ReminderId}", reminder.Id);
                }
            }
        }

        private async Task SendReminderNotification(TaskReminder reminder)
        {
            // For now, this is just a placeholder that logs the notification
            // In a real implementation, this would send emails, push notifications, etc.
            var reminderTypeText = reminder.Type switch
            {
                ReminderType.OneDayBefore => "one day before",
                ReminderType.OneHourBefore => "one hour before",
                ReminderType.Overdue => "overdue",
                _ => "unknown"
            };

            _logger.LogInformation("REMINDER: Task '{TaskTitle}' (ID: {TaskId}) is due {ReminderType}. Scheduled: {ScheduledDate}",
                reminder.TaskItem?.Title ?? "Unknown Task", 
                reminder.TaskItemId, 
                reminderTypeText, 
                reminder.ScheduledDate);

            // Simulate async notification sending
            await Task.Delay(100);
        }
    }
}