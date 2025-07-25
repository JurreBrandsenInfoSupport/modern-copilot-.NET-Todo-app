### Context
#### Project Structure
- The project uses a **feature slicing architecture**.
- Each feature resides in a folder in the application layer, named in the format: `[3-letter code][3-digit number][FeatureName]`.
- Each folder contains:
  - A feature file: `[FeatureName]Feature.cs`
  - A feature setup file: `[FeatureName]FeatureSetup.cs`
- Domain and UI language is in British English.

#### Database setup
- The database uses Entity Framework Core with postgress
- Related database entities are loosly coupled. Not every relation is configured in the models.
- The database uses a custom `AppDbContext` class.
- Enities are located in src/Domain/

#### Feature Setup
A standard feature setup file looks like this:
```csharp
public class [Featurename]FeatureSetup : IFeatureSetup
{
    public void SetupFeature(IFeatureBuilder builder)
    {
        builder.AddApiEndpoint<[Featurename]Endpoint>();
        builder.AddMassTransit()
               .WithMediatorConsumer<[Featurename]QueryHandler>();
    }
}
```

#### Feature File
Rules:
- The requests always follow the mediator pattern from the example.
- Mediator messages always have a query and response object.
- All requests return content if successfull.
- Feature files contain only one endpoint.
- Feature files are locateded in the src/Application folder
- The exception handling should always only return badrequest and match on innerexception.
- Database operations occur in the feature file.
- The type of the mediator is called Query when it is a Get-request and Command when it is a command.

Here is an example:
```csharp
namespace ProjectNamespace.Features.FeatureModule;

public class FeatureEndpoint : IApiEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("feature-route", async (IScopedMediator mediator) =>
        {
            try
            {
                var response = await mediator.Send<FeatureQuery, IEnumerable<FeatureResponse>>(new FeatureQuery());
                return Results.Ok(response);
            }
            catch (Exception e) when (e.InnerException is InvalidOperationException)
            {
                return Results.BadRequest(e.InnerException.Message);
            }
        })
        .WithName("FeatureOperationName")
        .Produces<FeatureDto>()
        .Produces<BadRequest<string>>()
        .RequireUserAuthorization();
    }
}

public record FeatureQuery();

public class FeatureQueryHandler : IConsumer<FeatureQuery>
{
    private readonly ApplicationDbContext _dbContext;

    public FeatureQueryHandler(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<FeatureQuery> context)
    {
        await context.RespondAsync();
    }
}
```

#### Testing
- Each feature has its own integration tests.
- Tests use:
- **Testcontainers** for API testing.
- **Builder pattern** for test model setup.
- **NSubstitute** for mocking.
- Setup methods from base class are executed.
- Feature Tests are located in tests/Application.Tests folder
- There is a TestDbContext for setting up entities and a separate DbContext for assertions.

A standard test class looks like this:
```csharp
namespace ProjectNamespace.Tests.FeatureGroup.Features.FeatureModule;

[TestClass]
public class FeatureEndpointTests : BaseAuthApiIntegrationTest
{
    [ClassInitialize]
    public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

    [ClassCleanup]
    public static async Task Cleanup() => await TestClassCleanup();

    [TestInitialize]
    public void TestInitialize()
    {
        IntegrationTestInitialize<WebUIAssemblyLocator>(
            [typeof(FeatureSetup)]);
    }

    [TestMethod]
    public async Task Feature_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        // Create entities with builder pattern
        using var context = GetTestDbContext();
        await context.AddEntities(entities);

        // Act
        var response = await TestClient.PutAsJsonAsync($"/test-route", new { });

        // Assert
        // Separate context for assertion
        var dbContext = GetDbContext();
        dbContext.Entity.FirstOrDefault(x => x.Id == id).Should().NotBeNull();

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<FeatureResponse>();
        result!.DTO.Should().NotBeNull();
        result.DTO.Ean.Should().Be(...);
        result.DTO.Source.Should().Be(...);
    }
}
```

### Prompt
Using the context above, perform the following task:

