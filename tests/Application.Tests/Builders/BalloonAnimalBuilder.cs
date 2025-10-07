using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class BalloonAnimalBuilder
    {
        private BalloonAnimal _balloonAnimal;

        private BalloonAnimalBuilder()
        {
            _balloonAnimal = new BalloonAnimal
            {
                Id = 1,
                AnimalType = "dog",
                Colour = "red",
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };
        }

        public static BalloonAnimalBuilder Create() => new();

        public BalloonAnimalBuilder WithId(int id)
        {
            _balloonAnimal.Id = id;
            return this;
        }

        public BalloonAnimalBuilder WithAnimalType(string animalType)
        {
            _balloonAnimal.AnimalType = animalType;
            return this;
        }

        public BalloonAnimalBuilder WithColour(string colour)
        {
            _balloonAnimal.Colour = colour;
            return this;
        }

        public BalloonAnimalBuilder WithCreatedAt(DateTime createdAt)
        {
            _balloonAnimal.CreatedAt = createdAt;
            return this;
        }

        public BalloonAnimalBuilder AsCompleted()
        {
            _balloonAnimal.IsCompleted = true;
            return this;
        }

        public BalloonAnimal Build() => _balloonAnimal;
    }
}