using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.TSK002TaskDueDates
{
    public static class TaskDueDatesFeatureSetup
    {
        public static IServiceCollection AddTaskDueDatesFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<UpdateTaskCommand, TaskItem>, UpdateTaskHandler>();
            services.AddScoped<IRequestHandler<GetTasksWithDueDatesQuery, List<TaskItem>>, GetTasksWithDueDatesHandler>();
            return services;
        }
    }
}