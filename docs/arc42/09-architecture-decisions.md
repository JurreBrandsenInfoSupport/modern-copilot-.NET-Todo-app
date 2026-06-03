# 9. Architecture Decisions

## Decision Log

Key architectural decisions are recorded here. For detailed ADRs, see the `docs/implementation-plans/` folder.

### ADR-1: Feature Slicing over Layered Architecture

**Context:** Traditional layered architectures scatter feature logic across multiple projects.  
**Decision:** Adopt vertical feature slicing with folders named `[3-letter][3-digit][Name]`.  
**Consequences:** Each feature is self-contained. Adding a feature requires no modification of existing code. Trade-off: slight duplication of patterns across features.

### ADR-2: CQRS via MediatR (In-Process)

**Context:** Need to separate read and write concerns without the complexity of event sourcing.  
**Decision:** Use MediatR for in-process command/query dispatch.  
**Consequences:** Handlers are independently testable. No network overhead. Single database for reads and writes (no eventual consistency complexity).

### ADR-3: Monolithic Deployment

**Context:** The application is a demonstration/learning project.  
**Decision:** Keep all features in a single deployable unit.  
**Consequences:** Simplified deployment and debugging. Feature slicing prepares for future extraction into microservices if needed.

### ADR-4: JWT Self-Issued Tokens

**Context:** Need authentication without external identity provider dependency.  
**Decision:** Issue JWT tokens internally using symmetric HMAC-SHA256 keys.  
**Consequences:** Simple setup, no external dependencies. Not suitable for multi-service architectures without shared key management.

### ADR-5: InMemory Database for Development

**Context:** Developers should be able to run the application without Docker or PostgreSQL installed.  
**Decision:** Use EF Core InMemoryDatabase by default; PostgreSQL via Docker Compose for production-like environments.  
**Consequences:** Fast startup, zero dependencies for local dev. Risk of behavioural differences between InMemory and PostgreSQL.

### ADR-6: Global Exception Handling Middleware

**Context:** Need consistent error responses across all endpoints.  
**Decision:** Implement custom `ExceptionHandlingMiddleware` returning ProblemDetails.  
**Consequences:** Uniform error format (RFC 7807). All unhandled exceptions are logged and safely exposed.
