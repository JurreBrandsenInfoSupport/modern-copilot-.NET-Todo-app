namespace TodoApp.Application.CMT003Comments
{
    public static class CommentsFeatureSetup
    {
        public static IServiceCollection AddCommentsFeature(this IServiceCollection services)
        {
            services.AddScoped<MediatR.IRequestHandler<AddCommentCommand, Domain.Entities.Comment>, AddCommentHandler>();
            services.AddScoped<MediatR.IRequestHandler<GetCommentsQuery, List<Domain.Entities.Comment>>, GetCommentsHandler>();
            return services;
        }
    }
}
