using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.BAL004BalloonAnimals;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Builders;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.tests.Application.Tests.BAL004BalloonAnimals
{
    [TestClass]
    public class BalloonAnimalsFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(BalloonAnimalsFeatureSetup)]);
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var command = new CreateBalloonAnimalCommand("dog", "red");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<BalloonAnimal>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.AnimalType.Should().Be("dog");
                result.Colour.Should().Be("red");
                result.IsCompleted.Should().BeFalse();
                result.Id.Should().BeGreaterThan(0);
                result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
            }

            // Verify in database
            var dbContext = GetDbContext();
            var createdAnimal = dbContext.BalloonAnimals.FirstOrDefault(x => x.AnimalType == "dog" && x.Colour == "red");
            createdAnimal.Should().NotBeNull();
            createdAnimal!.AnimalType.Should().Be("dog");
            createdAnimal.Colour.Should().Be("red");
            createdAnimal.IsCompleted.Should().BeFalse();
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldCreateAnimalInDatabase_WhenValidRequest()
        {
            // Arrange
            var command = new CreateBalloonAnimalCommand("giraffe", "yellow");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);

            // Assert
            response.EnsureSuccessStatusCode();

            // Separate context for assertion
            var dbContext = GetDbContext();
            var animal = dbContext.BalloonAnimals.FirstOrDefault(x => x.AnimalType == "giraffe" && x.Colour == "yellow");
            animal.Should().NotBeNull();
            animal!.AnimalType.Should().Be("giraffe");
            animal.Colour.Should().Be("yellow");
            animal.IsCompleted.Should().BeFalse();
            animal.Id.Should().BeGreaterThan(0);
            animal.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldReturnBadRequest_WhenInvalidAnimalType()
        {
            // Arrange
            var command = new CreateBalloonAnimalCommand("dragon", "purple");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Invalid animal type");
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldReturnBadRequest_WhenEmptyColour()
        {
            // Arrange
            var command = new CreateBalloonAnimalCommand("dog", "");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Colour is required");
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldReturnBadRequest_WhenNullAnimalType()
        {
            // Arrange
            var command = new CreateBalloonAnimalCommand(null!, "blue");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("AnimalType").And.Contain("required");
        }

        [TestMethod]
        public async Task GetAllBalloonAnimals_ShouldReturnOk_WhenAnimalsExist()
        {
            // Arrange
            var animals = new[]
            {
                BalloonAnimalBuilder.Create().WithAnimalType("dog").WithColour("red").WithId(1).Build(),
                BalloonAnimalBuilder.Create().WithAnimalType("giraffe").WithColour("yellow").WithId(2).AsCompleted().Build(),
                BalloonAnimalBuilder.Create().WithAnimalType("elephant").WithColour("blue").WithId(3).Build()
            };

            using var context = GetTestDbContext();
            await context.AddEntities(animals);

            // Act
            var response = await TestClient.GetAsync("/api/balloon-animals");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<BalloonAnimal>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCountGreaterOrEqualTo(3);

                var dog = result.FirstOrDefault(a => a.AnimalType == "dog");
                dog.Should().NotBeNull();
                if (dog != null)
                {
                    dog.Colour.Should().Be("red");
                    dog.IsCompleted.Should().BeFalse();
                }

                var giraffe = result.FirstOrDefault(a => a.AnimalType == "giraffe");
                giraffe.Should().NotBeNull();
                if (giraffe != null)
                {
                    giraffe.Colour.Should().Be("yellow");
                    giraffe.IsCompleted.Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public async Task GetAllBalloonAnimals_ShouldReturnEmptyList_WhenNoAnimalsExist()
        {
            // Act
            var response = await TestClient.GetAsync("/api/balloon-animals");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<BalloonAnimal>>();
            result.Should().NotBeNull();
            result.Should().BeOfType<List<BalloonAnimal>>();
        }

        [TestMethod]
        public async Task CreateBalloonAnimal_ShouldAcceptAllValidAnimalTypes()
        {
            // Arrange
            var validAnimalTypes = new[] { "dog", "giraffe", "sword", "flower", "elephant", "rabbit", "swan" };

            // Act & Assert
            foreach (var animalType in validAnimalTypes)
            {
                var command = new CreateBalloonAnimalCommand(animalType, "blue");
                var response = await TestClient.PostAsJsonAsync("/api/balloon-animals/create", command);
                
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<BalloonAnimal>();
                result.Should().NotBeNull();
                result!.AnimalType.Should().Be(animalType);
            }
        }
    }
}