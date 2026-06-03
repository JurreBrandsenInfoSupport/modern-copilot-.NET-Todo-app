# 11. Risks and Technical Debt

## Identified Risks

| ID   | Risk                                    | Probability | Impact | Mitigation                                          |
|------|-----------------------------------------|-------------|--------|-----------------------------------------------------|
| R-1  | InMemory DB behavioural differences     | Medium      | Medium | Integration tests use Testcontainers with PostgreSQL |
| R-2  | Single symmetric JWT key                | High        | High   | Rotate keys; consider asymmetric keys for production |
| R-3  | No refresh token mechanism              | Medium      | Medium | Tokens expire after 60 min; user must re-authenticate|
| R-4  | No email verification on registration   | Low         | Low    | Acceptable for demo; add verification for production |
| R-5  | CORS allows all origins                 | Medium      | Medium | Restrict to known origins in production deployment   |

## Technical Debt

| ID   | Debt Item                               | Severity | Effort | Notes                                               |
|------|-----------------------------------------|----------|--------|-----------------------------------------------------|
| TD-1 | InMemory DB in development mode         | Medium   | Low    | Replace with PostgreSQL via Docker for local dev     |
| TD-2 | Hardcoded fallback JWT secret           | High     | Low    | Move to secure configuration (Azure Key Vault, etc.) |
| TD-3 | No password hashing (username-only auth)| High     | Medium | Add password field to User entity + bcrypt hashing   |
| TD-4 | No pagination on list endpoints         | Medium   | Low    | Add `skip`/`take` parameters to queries              |
| TD-5 | No database migrations                  | Medium   | Low    | Add EF Core migrations for schema management         |
| TD-6 | Missing input validation attributes     | Medium   | Low    | Add FluentValidation or DataAnnotations              |

## Recommendations

1. **Immediate:** Replace hardcoded JWT key with environment-sourced secret
2. **Short-term:** Add password-based authentication with bcrypt hashing
3. **Medium-term:** Implement refresh tokens and token revocation
4. **Long-term:** Consider external identity provider (e.g., Azure AD, Auth0) for multi-tenant scenarios
