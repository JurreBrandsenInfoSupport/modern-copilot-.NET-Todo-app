# ADR 013: Kubernetes Orchestration

## Status

Accepted

## Date

2026-06-03

## Context

The TodoApp has outgrown simple Docker Compose deployments for production environments. We need a container orchestration platform that provides:

- Automated scaling and self-healing
- Service discovery and load balancing
- Rolling updates with zero-downtime deployments
- Declarative infrastructure management
- Secrets and configuration management
- Health monitoring and readiness checks

The application already has Docker images for the API and frontend, health check endpoints (`/health` and `/health/ready`), and structured logging — all of which are prerequisites for Kubernetes deployment.

## Decision

We will adopt **Kubernetes** as the container orchestration platform with **Helm** as the package manager for templated deployments.

### Deployment Strategy

1. **Raw manifests** (`k8s/`) serve as a reference implementation and for simple `kubectl apply` workflows.
2. **Helm chart** (`charts/todoapp/`) provides a configurable, repeatable deployment mechanism with environment-specific values files.

### Architecture on Kubernetes

| Component | Kind | Replicas | Notes |
|-----------|------|----------|-------|
| API | Deployment | 2 | Stateless, horizontally scalable |
| Frontend | Deployment | 2 | Static nginx, horizontally scalable |
| PostgreSQL | StatefulSet | 1 | Persistent volume for data durability |
| Redis | Deployment | 1 | Caching layer, ephemeral |
| Ingress | Ingress (nginx) | — | Path-based routing: `/api` → API, `/` → Frontend |

### Key Design Choices

- **StatefulSet for PostgreSQL**: Ensures stable network identity and persistent storage via PVC.
- **ClusterIP services**: Internal-only communication; external access via Ingress controller.
- **Resource limits on all containers**: Prevents noisy-neighbour issues and enables cluster autoscaling.
- **Liveness and readiness probes**: Leverages existing `/health` and `/health/ready` endpoints for automated recovery.
- **Secrets via Kubernetes Secrets**: Base64-encoded with guidance to use external secret stores (e.g., Azure Key Vault, HashiCorp Vault) in production.

## Consequences

### Positive

- Production-ready deployment with automated scaling, health management, and rolling updates.
- Helm chart enables consistent deployments across environments (dev, staging, production).
- Infrastructure-as-code approach with version-controlled manifests.
- Built-in service discovery eliminates manual networking configuration.

### Negative

- Increased operational complexity compared to Docker Compose.
- Requires a running Kubernetes cluster (local: minikube/kind, cloud: AKS/EKS/GKE).
- Team needs Kubernetes and Helm knowledge for day-to-day operations.
- PostgreSQL in Kubernetes is suitable for development/staging; production may warrant a managed database service.

### Mitigations

- Docker Compose remains available for local development.
- Deploy script (`scripts/deploy-k8s.ps1`) simplifies the deployment workflow.
- Comprehensive documentation in README covers setup and common operations.
