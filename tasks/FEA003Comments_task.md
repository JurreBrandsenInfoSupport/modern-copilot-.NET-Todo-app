# Feature: Comments

Implement **Comments** functionality for the TodoApp, allowing users to add comments to task items for improved collaboration and context tracking, and ensure stability and correctness with integration tests.

## Examples

- A user can add a comment (text) to a specific `TaskItem`.
- Comments are tied to both the `TaskItem` and the `User` who created them.
- A GET endpoint retrieves all comments for a `TaskItem`.
- Comments are stored in the database with a timestamp and user reference.
- Tests:
    - User can successfully add a comment to a `TaskItem`.
    - Retrieving comments returns correct data.
    - Comments persist and are tied to the correct user and task.
    - Adding an empty comment fails with `BadRequest`.
    - Invalid operations return `BadRequest`.

## Documentation

- **Domain Entity**:
    - `Domain/Entities/Comment.cs`:
        - `Id`
        - `TaskItemId`
        - `UserId`
        - `Text`
        - `CreatedAt`

- **Feature Folder**:
    - `Application/CMT003Comments/`
        - `CommentsFeature.cs` (handles GET endpoint with custom Mediator, database operations).
        - `CommentsFeatureSetup.cs` (registers endpoint`).

- Uses:
    - **Entity Framework Core** with `AppDbContext`.
    - **Custom Mediator pattern** (Query + Response objects).
    - **MediatR** for consumer handling.

- Uses **British English** in domain and UI naming.
- Tests are located in:
    - `tests/Application.Tests/CMT003Comments/CommentsFeatureTests.cs`.

- Tests use:
    - `BaseAuthApiIntegrationTest` as the base class.
    - **Testcontainers** for containerized PostgreSQL.
    - **Builder pattern** for `TaskItem` and `User` creation.
    - **NSubstitute** for mocks if required.
    - `TestDbContext` for entity setup and a separate context for assertions.

## Other considerations

- Maintain **loosely coupled foreign key configurations** in EF Core.
- Exception handling:
    - Use `try/catch` in endpoints with `BadRequest` on `InnerException is InvalidOperationException`.
- Follow **feature slicing conventions**:
    - Folder name: `CMT003Comments`.
    - Files: `CommentsFeature.cs`, `CommentsFeatureSetup.cs`.
- Ensure tests:
    - Use `ClassInitialize` and `ClassCleanup` for lifecycle management.
    - Register `CommentsFeatureSetup` in `TestInitialize`
    - Follow clear `Arrange / Act / Assert` structure.
    - Use `EnsureSuccessStatusCode` and assert DTO fields.
    - Confirm database state after API calls.
- Use **British English** for all naming and test data.