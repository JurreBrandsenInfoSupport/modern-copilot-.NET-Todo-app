# ADR 0005: Feature Slicing Architecture

## Status

Accepted

## Context

Traditional layered architectures organise code by technical concern (Controllers, Services, Repositories), which scatters a single feature's implementation across multiple folders. This makes it difficult to understand a feature holistically, increases the risk of unintended coupling between features, and complicates navigation in larger codebases.

Vertical slice architecture organises code by feature or use case. Each slice contains everything needed to handle a specific request, from the endpoint definition through to database access. This approach prioritises cohesion within a feature over technical layer consistency.

We needed a clear and consistent naming convention to identify features and their domain area at a glance. The convention should support ordering, categorisation, and discoverability within the project structure.

## Decision

We will organise application code using vertical feature slices. Each feature resides in its own folder under `src/Application` following the naming convention `[3-letter domain code][3-digit sequence number][FeatureName]`. For example: `TSK001CreateTask`, `USR002LoginUser`, `CMT003AddComment`.

Each feature folder contains:
- `[FeatureName]Feature.cs` — endpoint logic, request/response types, and handler
- `[FeatureName]FeatureSetup.cs` — endpoint registration and mediator consumer configuration

## Consequences

**Positive:**
- Features are self-contained and independently understandable
- Easy to locate all code related to a specific feature
- Clear naming convention communicates domain area and purpose immediately
- Adding a new feature does not require modifying existing features
- Natural alignment with CQRS and mediator patterns
- Simplifies code reviews as changes are typically within a single folder

**Negative:**
- Some code duplication between features (e.g., similar validation logic)
- Shared concerns require careful placement to avoid breaking slice boundaries
- Naming convention requires team agreement and discipline to maintain
- New developers need to learn the convention before contributing

**Neutral:**
- Cross-cutting concerns (authentication, logging) still live in shared infrastructure
- The three-letter domain code acts as a lightweight bounded context indicator
- Feature sequence numbers provide natural ordering within a domain area
