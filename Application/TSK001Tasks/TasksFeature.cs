using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.TSK001Tasks
{
    public record GetTasksByUserQuery(int UserId) : IRequest<List<TaskItem>>;

    public class GetTasksByUserHandler : IRequestHandler<GetTasksByUserQuery, List<TaskItem>>
    {
        private readonly AppDbContext _db;
        public GetTasksByUserHandler(AppDbContext db) => _db = db;

        public async Task<List<TaskItem>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
            => await _db.Tasks.Where(t => t.UserId == request.UserId).ToListAsync(cancellationToken);
    }
    public record CreateTaskCommand(string Title, int UserId, DateTime? DueDate = null) : IRequest<TaskItem>;

    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        public CreateTaskHandler(AppDbContext db) => _db = db;

        public async Task<TaskItem> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null) throw new ArgumentException($"User with ID {request.UserId} not found.");
            
            var task = new TaskItem 
            { 
                Title = request.Title, 
                UserId = request.UserId, 
                User = user,
                DueDate = request.DueDate
            };
            
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync(cancellationToken);

            // Create reminders if due date is provided and in the future
            if (request.DueDate.HasValue && request.DueDate.Value > DateTime.UtcNow)
            {
                await CreateRemindersForTask(task.Id, request.DueDate.Value, cancellationToken);
            }

            return task;
        }

        private async Task CreateRemindersForTask(int taskId, DateTime dueDate, CancellationToken cancellationToken)
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
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public record GetAllTasksQuery() : IRequest<List<TaskItem>>;

    public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, List<TaskItem>>
    {
        private readonly AppDbContext _db;
        public GetAllTasksHandler(AppDbContext db) => _db = db;

        public async Task<List<TaskItem>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
            => await _db.Tasks.ToListAsync(cancellationToken);
    }

}
