# ADR 0006: JWT Bearer Authentication

## Status

Accepted

## Context

The application requires authentication to protect endpoints and identify users making requests. We needed to choose between session-based authentication, cookie-based authentication, and token-based authentication.

Session-based authentication stores session state on the server, which complicates horizontal scaling as sessions must be shared across instances via sticky sessions or a distributed cache. Cookie-based authentication ties the client to browser-based interactions and introduces CSRF vulnerabilities.

JWT (JSON Web Token) Bearer authentication is a stateless approach where the server issues a signed token containing claims about the user. The client includes this token in the `Authorization` header with each request. The server validates the token's signature and expiry without needing to query a session store, making it inherently scalable.

Our application needs to support multiple client types including SPAs, mobile applications, and API consumers. A stateless token-based approach provides maximum flexibility across all these scenarios.

## Decision

We will use JWT Bearer tokens for authentication. Tokens will be issued upon successful login and validated on each request using ASP.NET Core's built-in JWT Bearer authentication middleware. Tokens will contain user identity claims and have a configurable expiry time.

## Consequences

**Positive:**
- Fully stateless — no server-side session storage required
- Horizontally scalable without session affinity or distributed caches
- Works seamlessly with any HTTP client (browsers, mobile apps, CLI tools)
- ASP.NET Core provides mature, well-tested JWT middleware
- Claims-based identity integrates naturally with authorisation policies
- Tokens are self-contained, reducing database lookups per request

**Negative:**
- Token revocation is difficult before natural expiry without a blocklist
- Tokens increase request payload size compared to session cookies
- Secret key management requires careful handling in production
- Token theft grants access until expiry if not properly secured

**Neutral:**
- Refresh token strategy needed for long-lived sessions
- Token expiry duration is a trade-off between security and user experience
- HTTPS is mandatory to prevent token interception in transit
