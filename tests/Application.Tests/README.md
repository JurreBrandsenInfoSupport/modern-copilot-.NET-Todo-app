# TodoApp Tests

This test project provides comprehensive integration tests for the TodoApp API following the specified testing patterns.

## Features Tested

### TSK001Tasks - Tasks Feature
- **CreateTask**: Tests task creation via POST `/api/tasks`
- **GetAllTasks**: Tests retrieving all tasks via GET `/api/tasks`
- Database validation and assertion separation

### USR002Users - Users Feature
- **RegisterUser**: Tests user registration via POST `/api/users`
- **GetAllUsers**: Tests retrieving all users via GET `/api/users`
- Database validation and assertion separation

## Test Architecture

### Infrastructure
- **BaseAuthApiIntegrationTest**: Base class for all integration tests
- **TestWebApplicationFactory**: Configures in-memory database for testing
- **TestDbContext**: Separate context for test data setup
- Integration with Entity Framework Core In-Memory provider

### Builder Pattern
- **TaskItemBuilder**: Fluent builder for creating test TaskItem entities
- **UserBuilder**: Fluent builder for creating test User entities

### Test Structure
Each test class follows the standard pattern:
```csharp
[TestClass]
public class [Feature]Tests : BaseAuthApiIntegrationTest
{
    [ClassInitialize] - Setup factory
    [ClassCleanup] - Cleanup factory
    [TestInitialize] - Initialize per test

    [TestMethod] - Individual test methods
}
```

## Technologies Used
- **MSTest**: Testing framework
- **FluentAssertions**: Assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: ASP.NET Core testing infrastructure
- **EntityFrameworkCore.InMemory**: In-memory database for testing
- **NSubstitute**: Mocking framework (available for future mocking needs)
- **Testcontainers**: Available for integration testing (configured but not used in current simple setup)

## Test Patterns Implemented

1. **Separate Test and Assertion Contexts**: Uses `GetTestDbContext()` for setup and `GetDbContext()` for assertions
2. **Builder Pattern**: Fluent builders for creating test entities with readable, maintainable test data
3. **Integration Testing**: Full HTTP request/response testing through TestClient
4. **Database Validation**: Verifies data persistence and retrieval
5. **Setup/Teardown**: Proper test lifecycle management

## Running Tests

```bash
cd tests/Application.Tests
dotnet test
```

## Example Test Pattern

```csharp
[TestMethod]
public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
{
    // Arrange
    var command = new CreateTaskCommand("Test Task");

    // Act
    var response = await TestClient.PostAsJsonAsync("/api/tasks", command);

    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<TaskItem>();
    result!.Title.Should().Be("Test Task");

    // Separate context for assertion
    var dbContext = GetDbContext();
    var task = dbContext.Tasks.FirstOrDefault(x => x.Title == "Test Task");
    task.Should().NotBeNull();
}
```
