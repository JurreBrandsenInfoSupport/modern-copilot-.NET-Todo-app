# 8. Cross-cutting Concepts

## Logging (Serilog)

Structured logging is configured via `builder.Host.UseSerilog()` with configuration-driven setup. Request logging middleware (`UseSerilogRequestLogging()`) automatically logs every HTTP request with timing, status code, and path. Log output is machine-parseable and enrichable with contextual properties.

## Error Handling (ProblemDetails)

The `ExceptionHandlingMiddleware` in `API/Middleware/` provides global exception handling:

| Exception Type             | HTTP Status | Title                |
|---------------------------|-------------|----------------------|
| `ArgumentException`        | 400         | Bad Request          |
| `KeyNotFoundException`     | 404         | Not Found            |
| `InvalidOperationException`| 409         | Conflict             |
| All others                 | 500         | Internal Server Error|

Responses conform to [RFC 7807](https://tools.ietf.org/html/rfc7807) with `application/problem+json` content type.

## Authentication (JWT Bearer)

- **Algorithm:** HMAC-SHA256 symmetric key
- **Token lifetime:** 60 minutes
- **Validation:** Issuer, audience, lifetime, and signing key are all validated
- **Claims:** Subject (user ID), name (username), role
- **Endpoint:** `POST /api/auth/token` issues tokens for registered users

## Rate Limiting

Fixed-window rate limiter configured in `Program.cs`:

- **Limit:** 100 requests per minute per client
- **Queue:** 2 additional requests queued (oldest-first processing)
- **Rejection:** HTTP 429 Too Many Requests

## CORS

A `DefaultCorsPolicy` is configured allowing any origin, method, and header. This is suitable for development; production deployments should restrict origins.

## Health Checks

Two endpoints are registered:
- `/health` — basic self-check confirming the application is running
- `/health/ready` — readiness probe running all registered checks

Both return JSON with individual check status, suitable for container orchestrator probes.

## API Versioning

Configured via `Asp.Versioning` with three simultaneous version readers:
- URL segment (`/api/v1/...`)
- Query string (`?api-version=1.0`)
- Header (`X-Api-Version: 1.0`)

Default version is `1.0`, assumed when unspecified.

## Frontend Architecture

The React SPA implements several cross-cutting concerns:

### JWT Token Management

The `AuthContext` provides centralised authentication state management:
- Stores the JWT token in memory (not localStorage) for security
- Automatically includes the `Authorization: Bearer <token>` header on all authenticated API requests
- Handles token expiry and redirects to the login page when a 401 response is received

### Server State (TanStack Query)

TanStack Query manages all server state with:
- Automatic background refetching for stale data
- Optimistic updates for responsive UI
- Request deduplication (multiple components requesting the same data trigger a single API call)
- Cache invalidation on mutations

### Styling (Tailwind CSS)

Tailwind CSS provides a consistent design language:
- Utility-first approach eliminates custom CSS sprawl
- Responsive design built into the class system
- Consistent spacing, colour, and typography scales across all pages
