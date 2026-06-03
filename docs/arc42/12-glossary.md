# 12. Glossary

| Term                  | Definition                                                                                              |
|-----------------------|---------------------------------------------------------------------------------------------------------|
| **Feature Slice**     | A vertical slice of functionality containing all layers (handler, setup, endpoint) in a single folder   |
| **CQRS**             | Command Query Responsibility Segregation — separating read operations (queries) from write operations (commands) |
| **MediatR**          | A .NET library implementing the mediator pattern for in-process message dispatch                         |
| **ProblemDetails**    | RFC 7807 standard for machine-readable error responses in HTTP APIs                                     |
| **Bearer Token**      | An authentication scheme where the client presents a token (typically JWT) in the Authorization header  |
| **JWT**              | JSON Web Token — a compact, URL-safe token format for securely transmitting claims between parties       |
| **EF Core**          | Entity Framework Core — Microsoft's object-relational mapper for .NET                                   |
| **DbContext**        | The primary class in EF Core that coordinates database operations and change tracking                    |
| **Testcontainers**   | A library that provides lightweight, disposable containers for integration testing                       |
| **Rate Limiting**    | Controlling the number of requests a client can make within a time window to prevent abuse              |
| **Health Check**     | An endpoint that reports the operational status of an application and its dependencies                   |
| **API Versioning**   | Supporting multiple versions of an API simultaneously to avoid breaking existing clients                 |
| **Serilog**          | A structured logging library for .NET that supports sinks, enrichers, and configuration-driven setup    |
| **Minimal API**      | A simplified approach in ASP.NET Core for defining HTTP endpoints without controllers                   |
| **Docker Compose**   | A tool for defining and running multi-container Docker applications using a YAML file                   |
| **CORS**             | Cross-Origin Resource Sharing — a mechanism that allows restricted resources to be requested from another domain |
| **NSubstitute**      | A .NET mocking framework for creating test doubles in unit and integration tests                        |
| **Swagger/OpenAPI**  | A specification and toolset for describing, producing, and consuming RESTful APIs                       |
| **Fixed Window**     | A rate limiting algorithm that counts requests in fixed time intervals (e.g., per minute)               |
| **Composition Root** | The single location in an application where dependency injection is configured (`Program.cs`)           |
