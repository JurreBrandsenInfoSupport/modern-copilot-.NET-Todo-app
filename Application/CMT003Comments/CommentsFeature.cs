using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.CMT003Comments
{
    // Command to add a comment
    public record AddCommentCommand(int TaskItemId, int UserId, string Text) : IRequest<Comment>;

    // Handler for adding a comment
    public class AddCommentHandler : IRequestHandler<AddCommentCommand, Comment>
    {
        private readonly AppDbContext _db;
        public AddCommentHandler(AppDbContext db) => _db = db;

        public async Task<Comment> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
                throw new ArgumentException("Comment text cannot be empty.");

            var task = await _db.Tasks.FindAsync([request.TaskItemId], cancellationToken);
            var user = await _db.Users.FindAsync([request.UserId], cancellationToken);
            if (task == null || user == null)
                throw new ArgumentException("Invalid TaskItemId or UserId.");

            var comment = new Comment
            {
                TaskItemId = request.TaskItemId,
                UserId = request.UserId,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync(cancellationToken);
            return comment;
        }
    }

    // Query to get comments for a task
    public record GetCommentsQuery(int TaskItemId) : IRequest<List<Comment>>;

    // Handler for retrieving comments
    public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, List<Comment>>
    {
        private readonly AppDbContext _db;
        public GetCommentsHandler(AppDbContext db) => _db = db;

        public async Task<List<Comment>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            return await _db.Comments
                .Where(c => c.TaskItemId == request.TaskItemId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
