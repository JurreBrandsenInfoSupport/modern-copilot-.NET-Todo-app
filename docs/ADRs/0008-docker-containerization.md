# ADR 0008: Docker Containerisation

## Status

Accepted

## Context

Consistent environments across development, testing, and production are critical for reducing "it works on my machine" issues. We needed a strategy for packaging and running the application that ensures reproducibility and simplifies deployment.

Virtual machines provide isolation but are heavyweight and slow to start. Platform-as-a-Service offerings vary between cloud providers, creating vendor lock-in. Containers offer a lightweight, standardised packaging format that runs consistently across any environment with a container runtime.

Docker is the industry-standard containerisation platform with extensive tooling support, a large image registry (Docker Hub), and native integration with all major CI/CD systems and cloud platforms. Docker Compose enables multi-container local development environments, allowing developers to spin up the application alongside its dependencies (PostgreSQL) with a single command.

Multi-stage Docker builds separate the build environment from the runtime environment, producing minimal production images that contain only the compiled application and its runtime dependencies.

## Decision

We will containerise the application using Docker with multi-stage builds. A `Dockerfile` will define the build and runtime stages. Docker Compose will orchestrate local development environments including the application and PostgreSQL database.

## Consequences

**Positive:**
- Consistent environments from development through to production
- Docker Compose provides one-command local development setup
- Multi-stage builds produce small, secure production images
- Container images are portable across all major cloud platforms
- Simplifies CI/CD pipelines with predictable build artefacts
- Easy to add additional services (Redis, message queues) to the local stack

**Negative:**
- Developers must install and understand Docker
- Container networking adds complexity for local debugging
- Image layer caching strategies require understanding to optimise build times
- Running containers add resource overhead compared to native execution

**Neutral:**
- Container orchestration (Kubernetes, ECS) decisions deferred to deployment phase
- Base image selection impacts security posture and must be kept updated
- Docker Desktop licensing may apply for enterprise development teams
