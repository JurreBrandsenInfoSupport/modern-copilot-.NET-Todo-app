# 4. Solution Strategy

## Key Architectural Decisions

| Decision                         | Rationale                                                                |
|----------------------------------|--------------------------------------------------------------------------|
| CQRS with MediatR               | Separates queries from commands; enables independent scaling and testing |
| Feature Slicing                  | Each feature is self-contained with its own handlers and registration    |
| Entity Framework Core            | Rapid development, LINQ support, migration tooling                       |
| JWT Bearer Authentication        | Stateless auth suitable for APIs; no session storage needed              |
| Thin Controllers + MediatR       | Controllers only route and return; logic lives in handlers               |
| ProblemDetails for errors        | RFC 7807 compliant error responses for consistent API behaviour          |
| Serilog structured logging       | Machine-parseable logs with contextual enrichment                        |

## Technology Choices

- **Runtime:** .NET 8 (LTS) — high performance, cross-platform, mature ecosystem
- **Web Framework:** ASP.NET Core Minimal APIs + Controllers hybrid
- **ORM:** Entity Framework Core with PostgreSQL provider
- **Mediator:** MediatR for in-process command/query dispatch
- **Auth:** Microsoft.AspNetCore.Authentication.JwtBearer
- **Logging:** Serilog with request logging middleware
- **API Docs:** Swashbuckle (Swagger/OpenAPI)
- **Containerisation:** Docker multi-stage build + Docker Compose

## Achieving Quality Goals

| Quality Goal    | Strategy                                                        |
|-----------------|----------------------------------------------------------------|
| Maintainability | Feature folders isolate change; adding features requires no modification of existing code |
| Testability     | DI + MediatR enable handler-level integration tests with Testcontainers |
| Security        | JWT validation on every request; rate limiting prevents abuse    |
| Observability   | Serilog request logging + health check endpoints                |
| Scalability     | Stateless design allows horizontal scaling behind a load balancer |
