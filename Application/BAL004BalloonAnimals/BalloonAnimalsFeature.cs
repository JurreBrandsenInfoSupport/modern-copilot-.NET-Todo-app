using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Infrastructure;

namespace TodoApp.Application.BAL004BalloonAnimals
{
    public record CreateBalloonAnimalCommand(string AnimalType, string Colour) : IRequest<BalloonAnimal>;

    public class CreateBalloonAnimalHandler : IRequestHandler<CreateBalloonAnimalCommand, BalloonAnimal>
    {
        private readonly AppDbContext _db;
        public CreateBalloonAnimalHandler(AppDbContext db) => _db = db;

        public async Task<BalloonAnimal> Handle(CreateBalloonAnimalCommand request, CancellationToken cancellationToken)
        {
            // Validate animal type
            var validAnimalTypes = new[] { "dog", "giraffe", "sword", "flower", "elephant", "rabbit", "swan" };
            if (string.IsNullOrWhiteSpace(request.AnimalType) || 
                !validAnimalTypes.Contains(request.AnimalType.ToLowerInvariant()))
            {
                throw new ArgumentException($"Invalid animal type. Valid types are: {string.Join(", ", validAnimalTypes)}");
            }

            // Validate colour
            if (string.IsNullOrWhiteSpace(request.Colour))
            {
                throw new ArgumentException("Colour is required");
            }

            var balloonAnimal = new BalloonAnimal 
            { 
                AnimalType = request.AnimalType.ToLowerInvariant(), 
                Colour = request.Colour.ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };
            
            _db.BalloonAnimals.Add(balloonAnimal);
            await _db.SaveChangesAsync(cancellationToken);
            return balloonAnimal;
        }
    }

    public record GetAllBalloonAnimalsQuery() : IRequest<List<BalloonAnimal>>;

    public class GetAllBalloonAnimalsHandler : IRequestHandler<GetAllBalloonAnimalsQuery, List<BalloonAnimal>>
    {
        private readonly AppDbContext _db;
        public GetAllBalloonAnimalsHandler(AppDbContext db) => _db = db;

        public async Task<List<BalloonAnimal>> Handle(GetAllBalloonAnimalsQuery request, CancellationToken cancellationToken)
            => await _db.BalloonAnimals.OrderByDescending(ba => ba.CreatedAt).ToListAsync(cancellationToken);
    }
}