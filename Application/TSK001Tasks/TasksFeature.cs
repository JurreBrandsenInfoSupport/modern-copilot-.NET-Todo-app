using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.TSK001Tasks
{
    public record CreateTaskCommand(string Title) : IRequest<TaskItem>;

    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskItem>
    {
        private readonly AppDbContext _db;
        public CreateTaskHandler(AppDbContext db) => _db = db;

        public async Task<TaskItem> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = new TaskItem { Title = request.Title };
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
