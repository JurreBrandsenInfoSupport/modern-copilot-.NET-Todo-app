# ADR 012: Redis Distributed Caching

## Status

Accepted

## Date

2026-06-03

## Context

The Todo application serves GET endpoints (`/api/v1/tasks`, `/api/v1/users`, `/api/v1/comments`) that repeatedly query the database for data that changes infrequently relative to the read frequency. As the application scales horizontally behind a load balancer, each instance would maintain its own isolated in-process cache, leading to inconsistent responses and wasted memory.

We need a distributed caching layer that:
- Reduces database load for frequently read data
- Provides consistent cache state across multiple application instances
- Supports cache invalidation when data changes
- Degrades gracefully if the cache is unavailable

## Decision

We will introduce **Redis** as a distributed caching layer using `Microsoft.Extensions.Caching.StackExchangeRedis` and a MediatR pipeline behavior pattern.

### Key design decisions:

1. **MediatR Pipeline Behaviors**: Caching is implemented as cross-cutting concerns via `CachingBehavior<TRequest, TResponse>` and `CacheInvalidationBehavior<TRequest, TResponse>`, keeping feature code clean.

2. **Marker Interfaces**: Queries opt-in to caching by implementing `ICacheableQuery` (providing cache key and TTL). Commands opt-in to invalidation by implementing `ICacheInvalidatingCommand` (providing keys to invalidate).

3. **TTL Strategy**: 30-second absolute expiration for tasks/users/comments queries. Short enough to ensure reasonable freshness, long enough to meaningfully reduce database load.

4. **Graceful Degradation**: Cache read/write failures are caught and logged as warnings. The application continues to function by falling back to the database.

5. **Testing Environment**: Tests use `IDistributedMemoryCache` (in-memory) to avoid requiring a Redis instance, while production uses the Redis-backed implementation.

6. **Health Check**: Redis is registered as a health check with `Degraded` failure status (not `Unhealthy`), reflecting that the app can function without it.

## Consequences

### Positive
- Reduced database load for read-heavy workloads
- Consistent caching across horizontally scaled instances
- Clean separation of concerns via pipeline behaviors
- No feature code changes needed to add caching to new queries
- Graceful degradation ensures availability even if Redis is down

### Negative
- Additional infrastructure dependency (Redis)
- Potential for stale reads within the TTL window
- Increased operational complexity (monitoring, backup, memory management)
- Serialisation/deserialisation overhead for cached objects

### Risks
- Cache stampede under high concurrency (mitigated by short TTL)
- Memory pressure on Redis if cache keys grow unbounded (mitigated by absolute expiration)
