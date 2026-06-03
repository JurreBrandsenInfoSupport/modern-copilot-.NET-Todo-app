# 5. Building Block View

## Level 0 — System Overview

```mermaid
graph LR
    Browser([Browser]) -->|HTTP :3000| Frontend[React SPA<br/>TypeScript + Vite + Tailwind]
    Frontend -->|REST/JSON| API[TodoApp API<br/>.NET 8]
    API -->|TCP :5432| DB[(PostgreSQL)]
```

## Level 1 — System Layers

```mermaid
graph TB
    subgraph "Frontend (React SPA)"
        Pages[Pages<br/>Login, Dashboard, Tasks, Users, Health]
        AuthCtx[AuthContext<br/>JWT Token Management]
        TQ[TanStack Query<br/>Server State & Caching]
    end

    subgraph "API Layer"
        Controllers[Controllers<br/>AuthController, TasksController,<br/>UsersController, CommentsController]
        Middleware[Middleware<br/>ExceptionHandlingMiddleware]
    end

    subgraph "Application Layer"
        TSK[TSK001Tasks<br/>TasksFeature + Setup]
        USR[USR002Users<br/>UsersFeature + Setup]
        CMT[CMT003Comments<br/>CommentsFeature + Setup]
    end

    subgraph "Domain Layer"
        Entities[Entities<br/>TaskItem, User, Comment]
    end

    subgraph "Infrastructure Layer"
        DbContext[AppDbContext]
        DB[(PostgreSQL)]
    end

    Pages --> AuthCtx
    Pages --> TQ
    TQ -->|REST/JSON| Controllers
    Controllers --> TSK
    Controllers --> USR
    Controllers --> CMT
    TSK --> Entities
    USR --> Entities
    CMT --> Entities
    TSK --> DbContext
    USR --> DbContext
    CMT --> DbContext
    DbContext --> DB
```

## Level 2 — Feature Folder Structure

Each feature follows a consistent internal structure:

| File                  | Responsibility                                      |
|-----------------------|-----------------------------------------------------|
| `*Feature.cs`         | Commands, Queries, and their MediatR Handlers       |
| `*FeatureSetup.cs`    | Service registration (DI) and endpoint mapping      |

### Feature Catalogue

| Code    | Feature   | Commands              | Queries                        |
|---------|-----------|----------------------|-------------------------------|
| TSK001  | Tasks     | `CreateTaskCommand`   | `GetAllTasksQuery`, `GetTasksByUserQuery` |
| USR002  | Users     | `RegisterUserCommand` | `GetAllUsersQuery`            |
| CMT003  | Comments  | (Create comment)      | (Get comments by task)        |

## Level 3 — Domain Entities

| Entity     | Properties                                    |
|------------|-----------------------------------------------|
| `TaskItem` | Id, Title, IsCompleted, UserId, User          |
| `User`     | Id, Username                                  |
| `Comment`  | Id, Content, TaskItemId, UserId (inferred)    |

## Cross-cutting Components

- **ExceptionHandlingMiddleware** — catches unhandled exceptions, returns ProblemDetails
- **Program.cs** — composition root wiring authentication, rate limiting, CORS, health checks, Serilog, and Swagger
