# Github Copilot Instructions

Follow these instructions when writing code or documentation for the project.

## Project Structure

The project uses **feature slicing architecture**:
- **Folders per feature under `src/Application`** named `[3-letter code][3-digit number][FeatureName]`.
- Each feature folder contains:
    - `[FeatureName]Feature.cs`: Defines the endpoint, logic, and database operations.
    - `[FeatureName]FeatureSetup.cs`: Registers endpoints and mediator consumers for the feature.
- The **domain models** are located under `src/Domain/Entities`.
- **Infrastructure** components (e.g., `AppDbContext`, entity configurations) are located under `src/Infrastructure`.
- The API layer is under `API/Controllers` (thin controllers if used, but feature folders primarily handle endpoint logic).
- Tests are located in `tests/Application.Tests`, mirroring the structure of the `Application` layer.

## Technology stack

- **.NET (ASP.NET Core)** using minimal API endpoints inside feature files.
- **Entity Framework Core with PostgreSQL**:
    - Uses a custom `ApplicationDbContext`.
    - Entities are loosely coupled (not all foreign keys explicitly configured).
- **Testcontainers** for integration testing with containerized databases.
- **NSubstitute** for mocking during testing.
- Uses **Builder pattern** for building entities in tests.

### Mapping on the structure:

- Database operations occur **inside feature files** using `ApplicationDbContext`.
- Endpoints are registered in `FeatureSetup`.
- Each feature handles:
    - Registration (`FeatureSetup.cs`).
    - Request handling (`Feature.cs`) with mediator pattern:
        - `Query` for GET.
        - `Command` for POST/PUT.
    - Exception handling:
        - Always uses `try/catch` with `BadRequest` returned on `InnerException is InvalidOperationException`.

## Testing the project

- Always create **integration tests for new features**.
- Tests are located in `tests/Application.Tests/<FeatureFolder>/<FeatureName>FeatureTests.cs`.
- Follow these principles:
    - 1 test case for the **expected use** (e.g., `ShouldReturnOk_WhenValidRequest`).
    - 1 **edge case** (e.g., no comments available).
    - 1 **failure case** (e.g., invalid request returns `BadRequest`).
- Use:
    - **Testcontainers** for containerized PostgreSQL.
    - Microsoft.VisualStudio.TestTools.UnitTesting;
    - `BaseAuthApiIntegrationTest` as the base class for integration tests.
    - `TestDbContext` for inserting entities.
    - A **separate context for assertions** to avoid tracking conflicts.
    - `FluentAssertions` for clear, expressive assertions.
- Ensure **British English** is used for naming and data within tests.