using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.TSK001Tasks
{
    public static class TasksFeatureSetup
    {
        public static IServiceCollection AddTasksFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateTaskCommand, TaskItem>, CreateTaskHandler>();
            services.AddScoped<IRequestHandler<GetAllTasksQuery, List<TaskItem>>, GetAllTasksHandler>();
            return services;
        }
    }
}
