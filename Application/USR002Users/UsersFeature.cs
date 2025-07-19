using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.USR002Users
{
    public record RegisterUserCommand(string Username) : IRequest<User>;

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, User>
    {
        private readonly AppDbContext _db;
        public RegisterUserHandler(AppDbContext db) => _db = db;

        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User { Username = request.Username };
            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            return user;
        }
    }

    public record GetAllUsersQuery() : IRequest<List<User>>;

    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly AppDbContext _db;
        public GetAllUsersHandler(AppDbContext db) => _db = db;

        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            => await _db.Users.ToListAsync(cancellationToken);
    }
}
