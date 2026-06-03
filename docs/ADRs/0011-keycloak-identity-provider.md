# ADR 011: Keycloak as OAuth2/OIDC Identity Provider

## Status

Accepted

## Context

The application previously used a simple custom JWT authentication scheme where the `AuthController` issued tokens directly based on a username lookup against the local database. This approach lacks:

- Centralised identity management
- Standards-compliant OAuth2/OpenID Connect flows
- Support for single sign-on (SSO)
- Proper password validation and credential management
- Token refresh and session management
- User self-service (password reset, account management)

As the application grows toward production readiness, a proper identity provider is required.

## Decision

We adopt **Keycloak** as the OAuth2/OIDC identity provider for the application.

### Key design choices:

1. **Keycloak** runs as a Docker service alongside the existing infrastructure.
2. A pre-configured realm (`todoapp`) is imported on startup with:
   - A public client (`todo-frontend`) for the SPA
   - A confidential client (`todo-api`) for backend token validation
   - A default demo user (`demo`/`demo`) with the `user` role
3. The ASP.NET Core backend validates tokens issued by Keycloak using the standard `JwtBearer` middleware with OIDC discovery.
4. The `/api/auth/token` endpoint is retained as a backend proxy to Keycloak's token endpoint (Resource Owner Password Grant), preserving the existing frontend contract.
5. The `TestAuthHandler` in integration tests continues to bypass real authentication, ensuring tests remain fast and independent of Keycloak.

## Consequences

### Positive

- Industry-standard OAuth2/OIDC authentication
- Centralised user management via Keycloak admin console
- Extensible: supports SSO, social login, MFA in future
- Token validation uses asymmetric keys (RSA) — no shared secrets in the API
- Clear separation of concerns between identity and application logic

### Negative

- Additional infrastructure dependency (Keycloak container)
- Increased startup time in development due to Keycloak initialisation
- Resource Owner Password Grant is used for demo simplicity but should be replaced with Authorization Code flow with PKCE for production SPAs

### Risks

- Keycloak availability becomes critical for authentication in production
- Realm configuration drift between environments if not managed as code
