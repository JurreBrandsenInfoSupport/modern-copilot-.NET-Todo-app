using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.Tests.Infrastructure;
using TodoApp.Infrastructure;

namespace TodoApp.tests.Application.Tests.Infrastructure
{
    public abstract class BaseAuthApiIntegrationTest
    {
        protected static TestWebApplicationFactory<Program> Factory = null!;
        protected HttpClient TestClient = null!;

        protected static async Task TestClassInitialize(TestContext _)
        {
            Factory = new TestWebApplicationFactory<Program>();
            await Task.CompletedTask;
        }

        protected static async Task TestClassCleanup()
        {
            Factory?.Dispose();
            await Task.CompletedTask;
        }

        protected void IntegrationTestInitialize<TAssemblyLocator>(Type[] featureSetupTypes)
        {
            TestClient = Factory.CreateClient();

            // Clean the database before each test
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        protected TestDbContext GetTestDbContext()
        {
            // Use the same database context as the API for test data setup
            var scope = Factory.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create TestDbContext with same configuration as AppDbContext
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
            var testDbContext = new TestDbContext(options);

            return testDbContext;
        }

        protected AppDbContext GetDbContext()
        {
            var scope = Factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }
    }

    // Assembly locator for the web UI
    public class WebUIAssemblyLocator
    {
    }
}
