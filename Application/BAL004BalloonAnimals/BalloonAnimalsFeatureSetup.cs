using MediatR;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.BAL004BalloonAnimals
{
    public static class BalloonAnimalsFeatureSetup
    {
        public static IServiceCollection AddBalloonAnimalsFeature(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<CreateBalloonAnimalCommand, BalloonAnimal>, CreateBalloonAnimalHandler>();
            services.AddScoped<IRequestHandler<GetAllBalloonAnimalsQuery, List<BalloonAnimal>>, GetAllBalloonAnimalsHandler>();
            return services;
        }
    }
}