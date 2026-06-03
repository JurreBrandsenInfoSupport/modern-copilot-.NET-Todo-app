# ADR 0007: Serilog for Structured Logging

## Status

Accepted

## Context

Effective logging is essential for debugging, monitoring, and auditing application behaviour in both development and production environments. Traditional text-based logging produces human-readable messages that are difficult to query, aggregate, and alert on at scale.

Structured logging captures log events as data with named properties rather than interpolated strings. This enables powerful querying (e.g., "show all requests where UserId = 42 and Duration > 500ms") and integrates well with modern observability platforms.

We evaluated the built-in `Microsoft.Extensions.Logging` abstraction, NLog, and Serilog. While the built-in abstraction provides a solid interface, it lacks the rich sink ecosystem and configuration flexibility of dedicated logging frameworks. Serilog is the most widely adopted structured logging library in the .NET ecosystem with over 100 community-maintained sinks.

Serilog supports configuration through `appsettings.json`, enabling sink changes without code modifications. Its enrichers add contextual information (machine name, thread ID, request correlation IDs) automatically to every log event.

## Decision

We will use Serilog as the logging implementation, integrated with the ASP.NET Core logging pipeline via `UseSerilog()`. Log configuration including sinks, minimum levels, and enrichers will be managed through `appsettings.json`. Structured log properties will be used instead of string interpolation.

## Consequences

**Positive:**
- Queryable structured log data enables powerful diagnostics
- Extensive sink ecosystem (Console, File, Seq, Elasticsearch, Application Insights)
- Configuration-driven setup allows environment-specific logging without code changes
- Enrichers automatically add contextual information to all events
- Message templates preserve both human readability and structured data
- Seamless integration with ASP.NET Core's logging abstractions

**Negative:**
- Additional NuGet packages required (Serilog + sinks + enrichers)
- Slight application startup cost for sink initialisation
- Team must learn Serilog's message template syntax
- Misconfigured sinks can silently drop log events

**Neutral:**
- Choosing appropriate log levels requires team conventions
- Log volume management and retention policies must be defined per environment
