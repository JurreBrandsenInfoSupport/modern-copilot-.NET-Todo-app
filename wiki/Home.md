# arc42 Architecture Documentation for modern-copilot-.NET-Todo-app

## 1. Introduction and Goals

This document describes the architecture of the modern-copilot-.NET-Todo-app, a .NET-based Todo application designed with modularity, testability, and modern practices.  
The system supports user management, task tracking, and commenting—all built on clean architecture principles, feature slicing, and robust integration testing.

### Quality Goals

- **Maintainability:** Feature-based structure, clear folder and file conventions.
- **Testability:** Integration tests using in-memory/Testcontainers, builder pattern, and separation of test concerns.
- **Extensibility:** Each feature is implemented as a distinct slice, easily extendable.
- **Error Handling:** Unified error patterns across endpoints.
- **Internationalization:** Uses British English in all domain and UI naming.

### Stakeholders

- Developers
- QA Engineers
- End users
- Project owner: [JurreBrandsenInfoSupport](https://github.com/JurreBrandsenInfoSupport)

---

## 2. Architecture Constraints

- .NET and C# only, using latest LTS version.
- Entity Framework Core for data access.
- MediatR for CQRS/Mediator patterns.
- Follows feature-sliced folder structure (`Application/TSK001Tasks`, `Application/USR002Users`, etc.).
- In-memory database for local/test, support for PostgreSQL with Testcontainers.
- British English naming in domain/UI.

---

## 3. System Scope and Context

### Business Context

The application provides:
- User registration and management.
- Task creation, assignment, and retrieval.
- Ability for users to comment on tasks for collaboration.

### Technical Context

- RESTful API built with ASP.NET Core.
- Feature endpoints registered via DI.
- Uses MediatR for handling commands and queries.
- Data persisted with EF Core.
- Integration tested using MSTest, FluentAssertions, and Testcontainers.

---

## 4. Solution Strategy

- **Feature Slicing:** Each business capability is a "feature", with its own handler, commands/queries, and tests.
- **CQRS:** Clear separation between read (Query) and write (Command) operations.
- **Testing:** Each feature has dedicated integration tests, with setup/teardown and builder patterns for test data.
- **Error Handling:** Try/catch in endpoints, BadRequest for validation/database errors.

---

## 5. Building Block View

### Level 1: System Context

- **API:** Entry point for HTTP requests.
- **Application Layer:** Contains features (e.g., Tasks, Users, Comments) as folders.
- **Domain Layer:** Business entities (e.g., TaskItem, User, Comment).
- **Infrastructure Layer:** Database context, EF Core configuration.

### Level 2: Features

**Example: Comments Feature**  
- Commands: `AddCommentCommand`  
- Queries: `GetCommentsQuery`  
- Handlers: `AddCommentHandler`, `GetCommentsHandler`  
- Entity: `Comment`  
- Test: `CommentsFeatureTests`

**Key files:**  
- `Application/CMT003Comments/CommentsFeature.cs`  
- `Domain/Entities/Comment.cs`  
- `tests/Application.Tests/CMT003Comments/CommentsFeatureTests.cs`

---

## 6. Runtime View

**Typical Flow:**  
1. API endpoint receives HTTP request (e.g., add comment).  
2. Endpoint invokes MediatR command/query.  
3. Handler executes logic, interacts with EF Core via `AppDbContext`.  
4. Result is returned to API endpoint, formatted and sent as HTTP response.

---

## 7. Deployment View

- Runs as a .NET Web API.
- Local/dev: In-memory database.
- CI/CD/test: Testcontainers with PostgreSQL.
- Production: Can be configured for real DB, scalable via containers.

---

## 8. Crosscutting Concepts

- **Error Handling:** All endpoints use try/catch and return `BadRequest` on domain/validation errors.
- **Testing:** Consistent structure using MSTest, FluentAssertions, separate setup/assertion contexts.
- **Authorization:** (Planned/Extendable) - endpoints can require user authorization.
- **Validation:** Input is validated in handlers, errors result in BadRequest.

---

## 9. Design Decisions

- **Feature folder structure:** Improves modularity and maintainability.
- **Custom Mediator pattern:** Adheres to CQRS, avoids tight coupling.
- **In-memory/Testcontainers for tests:** Enables fast, reliable integration tests.
- **British English:** Ensures internationalization consistency.

---

## 10. Quality Requirements

- All integration tests must pass (`dotnet test`).
- Each feature must have dedicated integration tests.
- Code and error handling should be consistent across features.

---

## 11. Risks and Technical Debt

- No explicit authentication/authorization yet (planned).
- Error handling focuses on validation/DB, but may need expansion for API security.
- Testcontainers are available, but not all tests may use them yet.

---

## 12. Glossary

- **Feature Slicing:** Organizing code by business feature rather than technical layer.
- **CQRS:** Command/Query Responsibility Segregation.
- **Testcontainers:** Library for integration testing with real databases in containers.

---

## References

- [Source Code](https://github.com/JurreBrandsenInfoSupport/modern-copilot-.NET-Todo-app)
- [Testing Patterns](https://github.com/JurreBrandsenInfoSupport/modern-copilot-.NET-Todo-app/blob/main/tests/Application.Tests/README.md)
- [Comments Feature Implementation](https://github.com/JurreBrandsenInfoSupport/modern-copilot-.NET-Todo-app/blob/main/tasks/FEA003Comments_task.md)

---

_Last updated: 2025-09-16_