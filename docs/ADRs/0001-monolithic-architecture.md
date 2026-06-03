# ADR 0001: Monolithic Architecture with Feature Slicing

## Status

Accepted

## Context

This application is a modern .NET Todo application designed as a demonstration and reference implementation. We needed to decide on the overall architectural style: microservices, modular monolith, or traditional layered monolith.

Microservices introduce significant operational complexity including service discovery, distributed transactions, inter-service communication, and independent deployment pipelines. For a demo application with a small team and a well-defined domain, this overhead is not justified.

A traditional layered monolith (Controllers → Services → Repositories) tends to produce highly coupled horizontal layers where a single feature change touches multiple layers and files scattered across the project.

A modular monolith with feature slicing offers the simplicity of a single deployment unit while maintaining clear internal boundaries between features. Each feature is self-contained within its own folder, making it straightforward to reason about, test, and potentially extract into a separate service in the future if scaling demands require it.

## Decision

We will use a modular monolithic architecture with vertical feature slicing. The application is deployed as a single unit with a single database. Features are organised into self-contained folders under `src/Application`, each encapsulating its own endpoint definitions, business logic, and data access.

## Consequences

**Positive:**
- Simple deployment and operational model with a single artefact
- Single database simplifies data consistency and transactions
- Feature folders provide clear boundaries and high cohesion
- Lower infrastructure costs and reduced DevOps complexity
- Features can be extracted into services later if needed

**Negative:**
- All features share the same process and resources
- Scaling must be done as a whole unit rather than per-feature
- Requires discipline to maintain feature boundary integrity
- A failure in one feature could potentially affect the entire application

**Neutral:**
- Team must agree on and enforce feature folder conventions
- Inter-feature communication happens via in-process calls rather than network boundaries
