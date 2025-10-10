using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.RMD001Reminders
{
    public record CreateReminderCommand(int TaskItemId, DateTime ScheduledDate, ReminderType Type) : IRequest<TaskReminder>;

    public class CreateReminderHandler : IRequestHandler<CreateReminderCommand, TaskReminder>
    {
        private readonly AppDbContext _db;
        public CreateReminderHandler(AppDbContext db) => _db = db;

        public async Task<TaskReminder> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
        {
            var task = await _db.Tasks.FindAsync(request.TaskItemId);
            if (task == null) throw new ArgumentException($"Task with ID {request.TaskItemId} not found.");

            var reminder = new TaskReminder
            {
                TaskItemId = request.TaskItemId,
                ScheduledDate = request.ScheduledDate,
                Type = request.Type
            };

            _db.TaskReminders.Add(reminder);
            await _db.SaveChangesAsync(cancellationToken);
            return reminder;
        }
    }

    public record GetPendingRemindersQuery() : IRequest<List<TaskReminder>>;

    public class GetPendingRemindersHandler : IRequestHandler<GetPendingRemindersQuery, List<TaskReminder>>
    {
        private readonly AppDbContext _db;
        public GetPendingRemindersHandler(AppDbContext db) => _db = db;

        public async Task<List<TaskReminder>> Handle(GetPendingRemindersQuery request, CancellationToken cancellationToken)
        {
            return await _db.TaskReminders
                .Include(r => r.TaskItem)
                .Where(r => !r.IsSent && r.ScheduledDate <= DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }
    }

    public record MarkReminderSentCommand(int ReminderId) : IRequest<TaskReminder>;

    public class MarkReminderSentHandler : IRequestHandler<MarkReminderSentCommand, TaskReminder>
    {
        private readonly AppDbContext _db;
        public MarkReminderSentHandler(AppDbContext db) => _db = db;

        public async Task<TaskReminder> Handle(MarkReminderSentCommand request, CancellationToken cancellationToken)
        {
            var reminder = await _db.TaskReminders.FindAsync(request.ReminderId);
            if (reminder == null) throw new ArgumentException($"Reminder with ID {request.ReminderId} not found.");

            reminder.IsSent = true;
            reminder.SentAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(cancellationToken);
            return reminder;
        }
    }

    public record GetRemindersByTaskQuery(int TaskItemId) : IRequest<List<TaskReminder>>;

    public class GetRemindersByTaskHandler : IRequestHandler<GetRemindersByTaskQuery, List<TaskReminder>>
    {
        private readonly AppDbContext _db;
        public GetRemindersByTaskHandler(AppDbContext db) => _db = db;

        public async Task<List<TaskReminder>> Handle(GetRemindersByTaskQuery request, CancellationToken cancellationToken)
        {
            return await _db.TaskReminders
                .Where(r => r.TaskItemId == request.TaskItemId)
                .OrderBy(r => r.ScheduledDate)
                .ToListAsync(cancellationToken);
        }
    }
}