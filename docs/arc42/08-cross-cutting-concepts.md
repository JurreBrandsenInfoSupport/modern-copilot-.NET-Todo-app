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
