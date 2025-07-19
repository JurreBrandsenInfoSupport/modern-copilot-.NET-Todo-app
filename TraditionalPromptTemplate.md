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
- The database uses a custom `ApplicationDbContext` class.
- Entities are configured in dedicated entity configuration classes in src/Infrastructure/Persistence/EntityTypeconfigurations.
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
- The mediator uses custom classes, not the Mediatr package.
- All requests return content if successfull.
- Feature files contain only one endpoint.
- Feature files are locateded in the src/Application folder
- The exception handling should always only return badrequest and match on innerexception.
- Database operations occur in the feature file.
- The type of the mediator is called Query when it is a Get-request and Command when it is a command.
Here is an example:
```csharp
namespace MyProject.Features.MDM032[Featurename];

public class [Featurename]Endpoint : IApiEndpoint
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("[FeatureRoute]", async (IScopedMediator mediator) =>
        {
            try
            {
                var response = await mediator.Send<[Featurename]Query, IEnumerable<[Featurename]Response>>(new [Featurename]Query());
                return Results.Ok(response);
            }
            catch (Exception e) when (e.InnerException is InvalidOperationException)
            {
                return Results.BadRequest(e.InnerException.Message);
            }
        })
        .WithName(nameof(MDM032HaalKenmerkenOp))
        .Produces<KenmerkDto>()
        .Produces<BadRequest<string>>()
        .RequireUserAuthorization();
    }
}

public record [Featurename]Query();

public class [Featurename]QueryHandler : IConsumer<[Featurename]Query>
{
    private readonly ApplicationDbContext _dbContext;

    public [Featurename]QueryHandler(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<[Featurename]Query> context)
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
namespace MyProject.Tests.Uitval.Features.UIT006[Featurename];

[TestClass]
public class [Featurename]Tests : BaseAuthApiIntegrationTest
{
    [ClassInitialize]
    public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

    [ClassCleanup]
    public static async Task Cleanup() => await TestClassCleanup();

    [TestInitialize]
    public void TestInitialize()
    {
        IntegrationTestInitialize<WebUIAssemblyLocator>(
            [typeof([Featurename]FeatureSetup)]);
    }

    [TestMethod]
    public async Task [Featurename]_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        // Create entities with builder pattern
        using var context = GetTestDbContext();
        await context.AddEntities(entities);

        // Act
        var response = await TestClient.PutAsJsonAsync($"/request-route", new { });

        // Assert
		//separate context for assertion
		var dbContext = GetDbContext();
		dbContext.Entity.FirstorDefault(x => x.Id == id).Should().NotBeNull();

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<[Featurename]Response>();
        result!.DTO.Should().NotBeNull();
        result.DTO.Ean.Should().Be(...);
        result.DTO.Bron.Should().Be(...);
    }
}
```

### Prompt
Using the context above, perform the following task:

