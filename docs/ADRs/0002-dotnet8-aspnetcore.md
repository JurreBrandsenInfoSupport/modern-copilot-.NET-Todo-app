# ADR 0002: .NET 8 and ASP.NET Core

## Status

Accepted

## Context

Selecting the right application framework and runtime is a foundational decision that impacts development velocity, performance, hiring, and long-term maintainability. We evaluated several options including Node.js with Express, Java with Spring Boot, and .NET with ASP.NET Core.

Key requirements included strong typing, high performance for web APIs, excellent tooling support, cross-platform capability, and a mature ecosystem with long-term support guarantees. The team has existing expertise in C# and the .NET ecosystem.

.NET 8 is a Long-Term Support (LTS) release from Microsoft, guaranteeing three years of support and security patches. ASP.NET Core consistently ranks among the top performers in independent benchmarks such as TechEmpower. The framework offers minimal APIs for lightweight endpoint definitions, built-in dependency injection, and comprehensive middleware pipeline support.

## Decision

We will use .NET 8 with ASP.NET Core as the application framework and runtime. We will leverage minimal API endpoints within feature files rather than traditional MVC controllers to reduce ceremony and keep endpoint definitions close to their handling logic.

## Consequences

**Positive:**
- LTS guarantees long-term security patches and stability
- Industry-leading performance for web API workloads
- Excellent cross-platform support (Linux, Windows, macOS)
- Rich tooling with Visual Studio, Rider, and VS Code
- Built-in dependency injection, configuration, and logging abstractions
- Large and active community with extensive NuGet package ecosystem
- Native AOT compilation options for future optimisation

**Negative:**
- Requires C# expertise which may limit the contributor pool compared to JavaScript
- Framework updates between major versions can require migration effort
- Minimal APIs are relatively new and some advanced scenarios still require MVC

**Neutral:**
- Microsoft stewardship provides stability but creates vendor association
- The .NET release cadence (annual) means staying current requires periodic upgrades
