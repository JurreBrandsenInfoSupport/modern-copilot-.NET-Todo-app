using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.RMD001Reminders
{
    public static class RemindersFeatureSetup
    {
        public static IServiceCollection AddRemindersFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateReminderCommand, TaskReminder>, CreateReminderHandler>();
            services.AddScoped<IRequestHandler<GetPendingRemindersQuery, List<TaskReminder>>, GetPendingRemindersHandler>();
            services.AddScoped<IRequestHandler<MarkReminderSentCommand, TaskReminder>, MarkReminderSentHandler>();
            services.AddScoped<IRequestHandler<GetRemindersByTaskQuery, List<TaskReminder>>, GetRemindersByTaskHandler>();
            return services;
        }
    }
}