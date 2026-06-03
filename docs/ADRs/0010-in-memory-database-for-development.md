# ADR 0010: In-Memory Database for Development and Testing

## Status

Accepted

## Context

During development and testing, developers need fast feedback cycles without requiring external infrastructure. Spinning up a full PostgreSQL instance for every test run or quick development iteration adds latency and complexity, particularly for new team members setting up their environment.

Entity Framework Core provides an InMemory database provider designed specifically for testing scenarios. It implements the `DbContext` interface identically to relational providers, allowing application code to run without modification against an in-memory store. This enables rapid test execution and zero-infrastructure local development.

However, the InMemory provider has known limitations: it does not enforce referential integrity, does not support transactions, and some LINQ queries that translate to SQL may behave differently. For scenarios requiring full relational behaviour, Testcontainers with PostgreSQL provides a containerised database that matches production exactly.

We needed a strategy that optimises for developer productivity in common scenarios while ensuring production-accurate testing where it matters most.

## Decision

We will use the EF Core InMemory database provider for rapid local development and lightweight unit testing where full relational semantics are not required. Integration tests and BDD scenarios will use Testcontainers with PostgreSQL to ensure production-accurate behaviour. Production deployments will always use PostgreSQL with proper connection string configuration.

## Consequences

**Positive:**
- Extremely fast test execution for scenarios not requiring relational features
- Zero infrastructure needed for basic development and unit testing
- New developers can run the application immediately without database setup
- Clear separation between fast feedback tests and full integration tests
- Reduces CI pipeline duration for the majority of test cases

**Negative:**
- InMemory provider does not enforce foreign keys or constraints
- Some EF Core features (raw SQL, database-specific functions) are unavailable
- Behavioural differences between InMemory and PostgreSQL can mask bugs
- Developers must understand which provider to use for which scenario

**Neutral:**
- Configuration switches between providers based on environment or test type
- Testcontainers bridges the gap for tests requiring full database fidelity
- The dual-provider approach requires clear documentation on when to use each
