using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.CMT003Comments
{
    public static class CommentsFeatureSetup
    {
        public static IServiceCollection AddCommentsFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<AddCommentCommand, Comment>, AddCommentHandler>();
            services.AddScoped<IRequestHandler<GetCommentsQuery, List<Comment>>, GetCommentsHandler>();
            return services;
        }
    }
}
