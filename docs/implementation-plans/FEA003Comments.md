# Comments Feature Requirements Document

## 0. Task Analysis

**Feature Goal:**
Implement Comments functionality for the TodoApp, allowing users to add comments to task items for improved collaboration and context tracking. Ensure stability and correctness with integration tests.

**Key Examples:**
- Users can add a comment (text) to a specific `TaskItem`.
- Comments are tied to both the `TaskItem` and the `User` who created them.
- A GET endpoint retrieves all comments for a `TaskItem`.
- Comments are stored in the database with a timestamp and user reference.
- Tests:
    - User can add a comment to a `TaskItem`.
    - Retrieving comments returns correct data.
    - Comments persist and are tied to the correct user and task.
    - Adding an empty comment fails with `BadRequest`.
    - Invalid operations return `BadRequest`.

**Documentation References:**
- Domain Entity: `Comment` (see below)
- Feature Folder: `Application/CMT003Comments/`
- Uses: Entity Framework Core, custom Mediator pattern, MediatR
- British English naming
- Tests: `tests/Application.Tests/CMT003Comments/CommentsFeatureTests.cs`

---

## 1. Codebase Analysis

### Existing Patterns to Mirror

- **Domain Entities:**
  - `TaskItem` and `User` classes in `Domain/Entities/`
- **DbContext:**
  - `AppDbContext` in `Infrastructure/AppDbContext.cs`
    - Uses `DbSet<TaskItem>` and `DbSet<User>`
- **Feature Setup:**
  - Tasks feature uses a folder structure:
    - `Application/TSK001Tasks/TasksFeature.cs` (MediatR handlers, queries, commands)
    - `Application/TSK001Tasks/TasksFeatureSetup.cs` (service registration)
- **API Controllers:**
  - `TasksController.cs` and `UsersController.cs` in `API/Controllers/`
    - Use MediatR for request handling
- **Testing:**
  - Integration tests in `tests/Application.Tests/TSK001Tasks/TasksFeatureTests.cs`
  - Test setup uses `BaseAuthApiIntegrationTest` and in-memory database

### Validation and Error Handling

- Use FluentAssertions for test validation
- Return `BadRequest` for invalid input (e.g., empty comment)
- Ensure correct user and task association

---

## 2. External Research

### Entity Framework Core Relationships

- [EF Core Relationships Documentation](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/)
- [Mapping Relationships in EF Core](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#mapping-relationships-in-ef-core)
- [One-to-Many Relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many)
- [Using Relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#using-relationships)

**Best Practices:**
- Use navigation properties for relationships (e.g., `TaskItem.Comments`, `User.Comments`)
- Use foreign keys for associations (`TaskItemId`, `UserId`)
- Use timestamps for tracking creation (`CreatedAt`)
- Use Fluent API or Data Annotations for configuration

**Common Pitfalls:**
- Not configuring relationships correctly (missing navigation or foreign key)
- Not validating input (empty or null comment text)
- Not handling concurrency or transaction issues

---

## 3. Context Gathering

### Domain Entity: Comment

Create `Domain/Entities/Comment.cs`:

```csharp
namespace TodoApp.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
```

### DbContext Update

Add to `AppDbContext`:

```csharp
public DbSet<Comment> Comments => Set<Comment>();
```

Configure relationships in `OnModelCreating` if needed.

### Feature Folder Structure

- `Application/CMT003Comments/CommentsFeature.cs`
  - MediatR handlers for AddComment, GetComments queries
- `Application/CMT003Comments/CommentsFeatureSetup.cs`
  - Service registration for handlers

### API Endpoint

- Add `CommentsController` in `API/Controllers/`
  - POST `/api/comments` to add a comment
  - GET `/api/comments?taskItemId={id}` to retrieve comments for a task

### Testing

- Create `tests/Application.Tests/CMT003Comments/CommentsFeatureTests.cs`
  - Test cases for add, retrieve, validation, and error scenarios

---

## Implementation Blueprint

### High-Level Approach

1. **Domain Model:**
   - Implement `Comment` entity with relationships to `TaskItem` and `User`
2. **DbContext:**
   - Add `DbSet<Comment>` and configure relationships
3. **Feature Handlers:**
   - Create MediatR commands/queries for adding and retrieving comments
   - Implement handlers in `CommentsFeature.cs`
4. **Service Registration:**
   - Register handlers in `CommentsFeatureSetup.cs`
5. **API Controller:**
   - Implement endpoints for adding and retrieving comments
6. **Validation:**
   - Ensure non-empty comment text, valid user/task references
7. **Testing:**
   - Integration tests for all scenarios

### Pseudocode Example

```csharp
// AddCommentCommand
public record AddCommentCommand(int TaskItemId, int UserId, string Text) : IRequest<Comment>;

// AddCommentHandler
public class AddCommentHandler : IRequestHandler<AddCommentCommand, Comment>
{
    // ...existing code...
    public async Task<Comment> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        // Check TaskItem and User exist
        // Create and save Comment
        // Return Comment
    }
}
```

### Error Handling

- Return `BadRequest` for empty text, invalid user/task
- Handle database exceptions gracefully

### Ordered Task List

1. Create `Comment` entity
2. Update `AppDbContext`
3. Implement MediatR commands/handlers
4. Register feature in DI
5. Add API controller
6. Write integration tests
7. Validate with `dotnet test`

### Validation and Testing Approach

- Use integration tests similar to `TasksFeatureTests.cs`
- Test all endpoints and error scenarios
- Use in-memory database for tests

---

## Validation Gates

- **Run tests:**
  `dotnet test`
- **Code quality:**
  Use IDE and build-in analyzers
- **Integration tests:**
  All tests in `tests/Application.Tests/CMT003Comments/CommentsFeatureTests.cs` must pass

---

## Quality Checklist

- [x] All necessary context for autonomous implementation
- [x] Validation gates that are executable
- [x] References to existing patterns and conventions
- [x] Clear, ordered implementation path
- [x] Comprehensive error handling documentation
- [x] Main flow and alternate scenarios covered
- [x] Specific code examples and file references

**Quality Score:** 9/10
**Rationale:**
The requirements document covers all necessary context, references, and implementation steps. It could be improved by including more detailed error handling examples and edge case scenarios, but is sufficient for a single-pass autonomous implementation.
