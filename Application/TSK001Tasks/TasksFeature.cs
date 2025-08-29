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
    public record CreateTaskCommand(string Title, int UserId) : IRequest<TaskItem>;

    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        public CreateTaskHandler(AppDbContext db) => _db = db;

        public async Task<TaskItem> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null) throw new ArgumentException($"User with ID {request.UserId} not found.");
            var task = new TaskItem { Title = request.Title, UserId = request.UserId, User = user };
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync(cancellationToken);
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

}
