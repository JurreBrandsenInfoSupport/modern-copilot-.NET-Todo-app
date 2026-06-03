# ADR 0003: PostgreSQL as Database

## Status

Accepted

## Context

The application requires a relational database to store structured data including tasks, users, and comments with referential integrity. We evaluated several database options including SQL Server, MySQL, PostgreSQL, and SQLite.

Key selection criteria included licensing costs, enterprise-grade reliability, ecosystem maturity, Entity Framework Core support quality, JSON data capabilities, and suitability for both development and production environments.

SQL Server offers excellent .NET integration but introduces licensing costs for production use. MySQL is widely used but has historical limitations with complex queries and its licensing model under Oracle ownership raises concerns. SQLite is excellent for embedded scenarios but lacks concurrent write support needed for web applications.

PostgreSQL is a fully open-source, enterprise-grade relational database with over 30 years of active development. It offers advanced features including JSONB columns, full-text search, CTEs, window functions, and excellent concurrency handling through MVCC. The Npgsql Entity Framework Core provider is mature, well-maintained, and supports all major EF Core features.

## Decision

We will use PostgreSQL as the primary relational database for this application. Data access will be managed through Entity Framework Core with the Npgsql provider. Database schema changes will be managed through EF Core migrations.

## Consequences

**Positive:**
- Zero licensing costs for any deployment scale
- Enterprise-grade reliability and ACID compliance
- Excellent EF Core support via the Npgsql provider
- Native JSONB support for semi-structured data when needed
- Proven scalability handling terabytes of data in production
- Strong community and extensive documentation
- Available as managed services on all major cloud providers

**Negative:**
- Requires running a separate database server (unlike SQLite)
- Configuration and tuning can be complex for production workloads
- Less integrated with Azure ecosystem compared to SQL Server

**Neutral:**
- Team needs PostgreSQL-specific knowledge for advanced features
- Docker Compose simplifies local development database provisioning
