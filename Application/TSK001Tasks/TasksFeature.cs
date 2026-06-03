using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Events;
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
    public record CreateTaskCommand(string Title, int UserId) : IRequest<TaskItem>;

    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        private readonly IPublisher _publisher;
        public CreateTaskHandler(AppDbContext db, IPublisher publisher) { _db = db; _publisher = publisher; }

        public async Task<TaskItem> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null) throw new ArgumentException($"User with ID {request.UserId} not found.");
            var task = new TaskItem { Title = request.Title, UserId = request.UserId, User = user };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync(cancellationToken);
            await _publisher.Publish(new TaskCreatedEvent(task.Id, task.Title, task.UserId), cancellationToken);
            return task;
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

    public record CompleteTaskCommand(int TaskId) : IRequest<TaskItem>;

    public class CompleteTaskHandler : IRequestHandler<CompleteTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        private readonly IPublisher _publisher;
        public CompleteTaskHandler(AppDbContext db, IPublisher publisher) { _db = db; _publisher = publisher; }

        public async Task<TaskItem> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _db.Tasks.FindAsync([request.TaskId], cancellationToken);
            if (task == null) throw new KeyNotFoundException($"Task with ID {request.TaskId} not found.");
            task.IsCompleted = true;
            await _db.SaveChangesAsync(cancellationToken);
            await _publisher.Publish(new TaskCompletedEvent(task.Id, task.Title, task.UserId), cancellationToken);
            return task;
        }
    }
}
