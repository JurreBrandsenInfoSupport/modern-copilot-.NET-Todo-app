using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.TSK002TaskDueDates
{
    public record UpdateTaskCommand(int Id, string? Title, DateTime? DueDate, bool? IsCompleted) : IRequest<TaskItem>;

    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        public UpdateTaskHandler(AppDbContext db) => _db = db;

        public async Task<TaskItem> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _db.Tasks.FindAsync(request.Id);
            if (task == null) throw new ArgumentException($"Task with ID {request.Id} not found.");

            var originalDueDate = task.DueDate;
            var wasCompleted = task.IsCompleted;

            // Update properties if provided
            if (!string.IsNullOrEmpty(request.Title))
                task.Title = request.Title;

            if (request.DueDate.HasValue)
                task.DueDate = request.DueDate.Value;

            if (request.IsCompleted.HasValue)
            {
                task.IsCompleted = request.IsCompleted.Value;
                if (request.IsCompleted.Value && !wasCompleted)
                {
                    task.CompletedAt = DateTime.UtcNow;
                }
                else if (!request.IsCompleted.Value && wasCompleted)
                {
                    task.CompletedAt = null;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            // Handle reminder updates
            if (request.DueDate.HasValue && request.DueDate != originalDueDate)
            {
                await UpdateRemindersForTask(task.Id, request.DueDate.Value, cancellationToken);
            }

            // Cancel reminders if task is completed
            if (request.IsCompleted.HasValue && request.IsCompleted.Value && !wasCompleted)
            {
                await CancelRemindersForTask(task.Id, cancellationToken);
            }

            return task;
        }

        private async Task UpdateRemindersForTask(int taskId, DateTime dueDate, CancellationToken cancellationToken)
        {
            // Remove existing reminders that haven't been sent
            var existingReminders = await _db.TaskReminders
                .Where(r => r.TaskItemId == taskId && !r.IsSent)
                .ToListAsync(cancellationToken);

            _db.TaskReminders.RemoveRange(existingReminders);

            // Create new reminders if due date is in the future
            if (dueDate > DateTime.UtcNow)
            {
                var reminders = new List<TaskReminder>();

                // One day before reminder
                var oneDayBefore = dueDate.AddDays(-1);
                if (oneDayBefore > DateTime.UtcNow)
                {
                    reminders.Add(new TaskReminder
                    {
                        TaskItemId = taskId,
                        ScheduledDate = oneDayBefore,
                        Type = ReminderType.OneDayBefore
                    });
                }

                // One hour before reminder
                var oneHourBefore = dueDate.AddHours(-1);
                if (oneHourBefore > DateTime.UtcNow)
                {
                    reminders.Add(new TaskReminder
                    {
                        TaskItemId = taskId,
                        ScheduledDate = oneHourBefore,
                        Type = ReminderType.OneHourBefore
                    });
                }

                if (reminders.Any())
                {
                    _db.TaskReminders.AddRange(reminders);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        private async Task CancelRemindersForTask(int taskId, CancellationToken cancellationToken)
        {
            var pendingReminders = await _db.TaskReminders
                .Where(r => r.TaskItemId == taskId && !r.IsSent)
                .ToListAsync(cancellationToken);

            _db.TaskReminders.RemoveRange(pendingReminders);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public record GetTasksWithDueDatesQuery(int? UserId, bool? OverdueOnly = false) : IRequest<List<TaskItem>>;

    public class GetTasksWithDueDatesHandler : IRequestHandler<GetTasksWithDueDatesQuery, List<TaskItem>>
    {
        private readonly AppDbContext _db;
        public GetTasksWithDueDatesHandler(AppDbContext db) => _db = db;

        public async Task<List<TaskItem>> Handle(GetTasksWithDueDatesQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Tasks.AsQueryable();

            if (request.UserId.HasValue)
            {
                query = query.Where(t => t.UserId == request.UserId.Value);
            }

            if (request.OverdueOnly.HasValue && request.OverdueOnly.Value)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && !t.IsCompleted);
            }

            return await query.OrderBy(t => t.DueDate).ToListAsync(cancellationToken);
        }
    }
}