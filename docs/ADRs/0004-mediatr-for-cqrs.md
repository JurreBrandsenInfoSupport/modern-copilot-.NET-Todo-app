# ADR 0004: MediatR for CQRS Pattern

## Status

Accepted

## Context

As the application grows, maintaining clean separation between the API layer and business logic becomes increasingly important. Without a clear pattern, controllers tend to accumulate business logic, making them difficult to test and maintain. We needed a mechanism to decouple request handling from endpoint definitions.

The Command Query Responsibility Segregation (CQRS) pattern separates read operations (queries) from write operations (commands), allowing each to be optimised independently. This pattern naturally aligns with our feature slicing architecture where each feature handles specific commands and queries.

We evaluated implementing CQRS manually with interfaces and handlers versus using the MediatR library. MediatR provides an in-process mediator implementation that routes requests to their corresponding handlers through a simple `IMediator.Send()` call. It also supports pipeline behaviours for cross-cutting concerns such as validation, logging, and transaction management.

## Decision

We will use MediatR to implement the CQRS pattern. Each feature will define its own `Query` (for GET operations) and `Command` (for POST/PUT operations) records with corresponding handlers. Pipeline behaviours will be used for cross-cutting concerns where appropriate.

## Consequences

**Positive:**
- Clean separation between endpoint definitions and business logic
- Each handler is independently testable with clear inputs and outputs
- Pipeline behaviours enable cross-cutting concerns without code duplication
- Consistent pattern across all features reduces cognitive load
- Handlers are naturally aligned with the single responsibility principle
- Easy to add new features without modifying existing code

**Negative:**
- Adds a layer of indirection that can make debugging slightly harder
- Additional NuGet dependency to maintain
- Simple CRUD operations may feel over-engineered with full mediator pattern
- Stack traces become deeper, which can complicate error diagnosis

**Neutral:**
- Team must understand the mediator pattern to contribute effectively
- Request/response objects add some boilerplate but improve explicitness
- MediatR is widely adopted in the .NET community, making it a familiar pattern for most developers
