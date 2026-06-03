# 6. Runtime View

## Scenario 1 — Create Task

```mermaid
sequenceDiagram
    participant Client
    participant TasksController
    participant MediatR
    participant CreateTaskHandler
    participant AppDbContext
    participant PostgreSQL

    Client->>TasksController: POST /api/tasks {title, userId}
    TasksController->>MediatR: Send(CreateTaskCommand)
    MediatR->>CreateTaskHandler: Handle(command)
    CreateTaskHandler->>AppDbContext: FindAsync(userId)
    AppDbContext->>PostgreSQL: SELECT user
    PostgreSQL-->>AppDbContext: User entity
    CreateTaskHandler->>AppDbContext: Add(TaskItem)
    CreateTaskHandler->>AppDbContext: SaveChangesAsync()
    AppDbContext->>PostgreSQL: INSERT task
    PostgreSQL-->>AppDbContext: OK
    CreateTaskHandler-->>MediatR: TaskItem
    MediatR-->>TasksController: TaskItem
    TasksController-->>Client: 201 Created {task}
```

## Scenario 2 — Authenticate (Get Token)

```mermaid
sequenceDiagram
    participant Client
    participant AuthController
    participant AppDbContext
    participant PostgreSQL

    Client->>AuthController: POST /api/auth/token {username}
    AuthController->>AppDbContext: FirstOrDefaultAsync(username)
    AppDbContext->>PostgreSQL: SELECT user WHERE username = ?
    PostgreSQL-->>AppDbContext: User entity
    AuthController->>AuthController: Generate JWT (HMAC-SHA256)
    AuthController-->>Client: 200 OK {token, expiresAt}
```

## Scenario 3 — Get Tasks by User (with Authentication)

```mermaid
sequenceDiagram
    participant Client
    participant JwtMiddleware
    participant RateLimiter
    participant TasksController
    participant MediatR
    participant GetTasksByUserHandler
    participant AppDbContext

    Client->>JwtMiddleware: GET /api/tasks?userId=1 [Bearer token]
    JwtMiddleware->>JwtMiddleware: Validate token
    JwtMiddleware->>RateLimiter: Pass request
    RateLimiter->>TasksController: Forward (within limit)
    TasksController->>MediatR: Send(GetTasksByUserQuery)
    MediatR->>GetTasksByUserHandler: Handle(query)
    GetTasksByUserHandler->>AppDbContext: Tasks.Where(userId).ToListAsync()
    AppDbContext-->>GetTasksByUserHandler: List<TaskItem>
    GetTasksByUserHandler-->>MediatR: List<TaskItem>
    MediatR-->>TasksController: List<TaskItem>
    TasksController-->>Client: 200 OK [tasks]
```

## Error Handling Flow

When an unhandled exception occurs, the `ExceptionHandlingMiddleware` catches it, logs it via Serilog, maps it to an appropriate HTTP status code, and returns a RFC 7807 `ProblemDetails` JSON response.
