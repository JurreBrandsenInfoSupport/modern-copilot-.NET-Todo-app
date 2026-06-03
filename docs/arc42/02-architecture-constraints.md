# 2. Architecture Constraints

## Technical Constraints

| Constraint             | Rationale                                                              |
|------------------------|------------------------------------------------------------------------|
| .NET 8 / ASP.NET Core  | LTS release with modern minimal API support and performance gains      |
| PostgreSQL 16          | Production database; chosen for reliability and JSON support            |
| Entity Framework Core  | ORM for database access; enables rapid development and migrations       |
| Docker                 | Containerised deployment ensures environment consistency                |
| Monolithic deployment  | Single deployable unit; appropriate complexity for a demo application   |

## Organisational Constraints

| Constraint                    | Rationale                                                         |
|-------------------------------|-------------------------------------------------------------------|
| Feature-slicing architecture  | Enforced folder structure (`[3-letter][3-digit][Name]`) for consistency |
| CQRS via MediatR              | Separates read and write concerns at the application layer        |
| Integration tests required    | Every feature must have tests using Testcontainers                |
| British English in tests      | Consistent naming and data conventions across the test suite      |

## Conventions

- Each feature lives in its own folder under `Application/` with a `Feature.cs` and `FeatureSetup.cs`
- Domain entities reside in `Domain/Entities/`
- Infrastructure (DbContext, configurations) lives in `Infrastructure/`
- Controllers in `API/Controllers/` remain thin, delegating to MediatR
- The `Program.cs` file is the composition root, wiring all cross-cutting concerns

## Development Environment

- InMemoryDatabase used for local development (no PostgreSQL required)
- PostgreSQL via Docker Compose for integration testing and production-like environments
- Swagger UI available at `/swagger` for interactive API exploration
