using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.USR002Users
{
    public static class UsersFeatureSetup
    {
        public static IServiceCollection AddUsersFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<RegisterUserCommand, User>, RegisterUserHandler>();
            services.AddScoped<IRequestHandler<GetAllUsersQuery, List<User>>, GetAllUsersHandler>();
            return services;
        }
    }

}
