# OAuth2/OpenID Connect Audit and Implementation Roadmap

## SECTION 1 — CURRENT STATE SUMMARY
The existing service is a lightweight ASP.NET Core Identity-based authentication API. It supports:

- Local user registration and login via `UserManager` / `SignInManager`.
- JWT generation using `TokenService` with RSA signing keys from configuration.
- Refresh tokens stored implicitly through `TokenInfo` entity but generation and validation are trivial (the same token returned without persistence checks).
- Basic endpoints in `AuthController` for login, register, refresh, update-password, and external login using ASP.NET Core social providers.
- Unit tests exercise service methods.

There is no OAuth2 or OpenID Connect protocol implementation; the service acts as a simple authentication provider issuing custom JWTs and handling internal refresh logic. Database migrations include a `TokenInfos` table but no client store, scopes, or consent models.

## SECTION 2 — GAP ANALYSIS TABLE
| Requirement | Status | Severity | Notes |
|-------------|--------|----------|-------|
| Authorization endpoint (`/authorize`) | Missing | Critical | No handling of auth code, PKCE, redirect validation. |
| Token endpoint (`/token`) | Missing | Critical | Only internal login endpoint exists. |
| Discovery endpoint | Missing | Critical | .well-known not exposed. |
| JWKS endpoint | Missing | High | Keys not published; manual config only. |
| Revocation endpoint | Missing | High | No token revocation support. |
| Introspection endpoint | Missing | High | Cannot inspect access tokens. |
| Consent/authorization UI | Missing | High | No model for user consent. |
| Client registration/store | Missing | Critical | No record of client ids/secrets. |
| Redirect URI validation | Missing | Critical | Unsafely trusts any returnUrl. |
| Scopes support | Missing | High | JWT claims do not encode scopes. |
| Grant types support | Only resource owner/password | Critical | No code, client credentials, etc. only login/password. |
| PKCE support | Missing | High | No code verifier/ challenge. |
| Refresh token rotation | Missing | High | Tokens are not rotated, persisted but not validated. |
| Userinfo endpoint (`/userinfo`) | Missing | High | No profile claims endpoint. |
| Session management | Minimal | Medium | ASP.NET Identity handled but not OIDC sessions. |
| External login broker | Partial | Recommended | Social login exists but not integrated into OAuth flows. |
| Key rotation | Basic config | High | RSA key static, rotation not automated. |
| Claims transformation | Basic | Medium | Only includes user name, email, roles. |
| Audience/Issuer enforcement | Partial | Medium | Tokens have issuer/audience but not enforced via validation logic. |
| Rate limiting/protection | None | High | No throttling. |
| CSRF/Replay protection | None on endpoints | High | No anti-forgery on auth endpoints. |
| Multi-tenant readiness | No | Medium | No tenant abstraction. |

## SECTION 3 — MISSING OAUTH/OIDC FEATURES
**Critical**
- `/authorize` and `/token` endpoints implementing OAuth2 flows (Authorization Code with PKCE, Client Credentials, Refresh).
- Client management: registration, secret hashing, redirect URI lists, grant type flags.
- Discovery (`/.well-known/openid-configuration`) and JWKS endpoints.
- Token revocation and introspection endpoints with persistence.
- Consent UI or backend to record user consent per client/scope.
- Scope support and inclusion in access tokens/ID tokens.
- Secure refresh token rotation and revocation.
- Userinfo endpoint returning standard OIDC claims.

**Required**
- PKCE validation.
- CSRF protection for authorization flow (state parameter). 
- Audience and issuer enforcement in validation policies.
- Rate limiting and brute-force protections.
- Revocation lists and refresh token blacklisting.

**Recommended**
- Device flow endpoint.
- Dynamic client registration (RFC 7591) support.
- Support for JWT access tokens and opaque tokens.
- Session management endpoints (logout, end-session). 
- Multi-tenant client isolation.

**Optional**
- Proof key for code exchange metadata.
- Token binding.
- FIDO2 / MFA integration.

## SECTION 4 — ARCHITECTURE PROBLEMS
- Single `AuthenticationService` mixes local login, registration and token generation without abstraction for OAuth flows.
- Token generation using `ITokenService` only produces JWT; no separation between access and ID tokens nor support for scopes or audiences beyond simple claims.
- Lack of a client store means identity server cannot track or validate clients; this is fundamental to OAuth.
- `TokenInfo` entity holds refresh tokens but validation is a no-op; refresh lifecycle not enforced.
- Controllers tightly coupled to application service; no middleware or pipeline for OAuth endpoints.
- External login logic embedded in service rather than through standardized `IExternalLoginProvider` abstraction.
- Absence of any configuration model for OAuth features like allowed grant types, CORS, etc.

## SECTION 5 — SECURITY RISKS
1. **Refresh tokens not validated or rotated** – high risk of token replay and persistent compromise. (Severity: Critical)
2. **Open redirect / redirect URI not validated** – enables phishing. (High)
3. **Authentication endpoints lack CSRF/anti-forgery** – vulnerable to authorization code injection when flows added. (High)
4. **Static RSA key with no rotation or exposure via JWKS** – key compromise leads to token forgery. (High)
5. **No rate limiting or brute force protection** – account enumeration and password guessing. (High)
6. **Tokens lack scopes and audience enforcement** – tokens may be used by unintended services. (Medium)
7. **Lack of supervised client secrets storage (possibly in plain text)** – leaks compromise all clients. (Medium)
8. **Missing discovery metadata** – clients may misconfigure and lead to insecure implementations. (Low)

## SECTION 6 — IMPLEMENTATION ROADMAP
1. **Define data models for clients, authorizations, and scopes**. Add entities and migrations for `OAuthClient`, `OAuthAuthorization`, `Scope`, `ClientScope`, `RefreshToken` with proper indexes.
2. **Implement a client store** (repository pattern) to read/write client credentials, allowed grant types, redirect URIs, etc.
3. **Extend `TokenService`** to support access/ID tokens with scopes, audiences, issuer and lifetimes; add signing key management with JWKS endpoint plus rotation logic.
4. **Implement OAuth endpoints**:
   - `/authorize` with support for response_type=code, state, scope, redirect_uri validation, PKCE, login & consent.
   - `/token` to handle code exchange, client credentials, refresh tokens, include client authentication (basic/POST).
5. **Add endpoints for discovery, JWKS, revocation, introspection, userinfo, end-session**.
6. **Enhance `AuthenticationService`** or create new service(s) to orchestrate flows; decouple from direct login and registration logic.
7. **Add consent UI or API** to capture user approvals; store them per user/client/scope.
8. **Secure refresh tokens**: store hashed refresh tokens, implement rotation and revocation, maintain refresh token logs for revocation endpoint.
9. **Implement CSRF, state parameter validation, and rate limiting** across auth endpoints. Use ASP.NET Core middleware or filters.
10. **Add configuration and startup** options for token lifetimes, allowed origins, supported grant types, etc.
11. **Write comprehensive unit/integration tests** covering all OAuth flows, error cases and security checks.
12. **Plan for multi-tenancy** by scoping clients and users to tenants.
13. **Evaluate scalability**: separate token issuance from user store; consider distributed caches (Redis) for authorization codes and tokens.
14. **Conduct security review & compliance testing** with OAuth2/OIDC certification tools.

## SECTION 7 — DATA MODEL CHANGES
- `OAuthClient` entity: Id, ClientId, ClientSecretHash, RedirectUris, GrantTypes, Scopes, IsConfidential, TokenEndpointAuthMethod.
- `Scope` entity: Name, Description, IsDefault, Resources.
- `OAuthAuthorization` entity: Id, UserId, ClientId, Scopes, GrantedAt, ConsentGiven.
- `OAuthCode` (or reuse authorization) with Code, PKCEChallenge, ExpiresAt, RedirectUri.
- `RefreshToken` entity: TokenHash, UserId, ClientId, ExpiresAt, RevokedAt, RotationCounter.
- Modify `TokenInfo` or replace with above refresh token structure; remove unnecessary plain refresh tokens.
- Add relationships between ApplicationUser and authorizations.

## SECTION 8 — NEW ENDPOINTS REQUIRED
- `GET /.well-known/openid-configuration` – discovery metadata.
- `GET /.well-known/jwks.json` – public keys.
- `GET /authorize` – authorization endpoint.
- `POST /token` – token endpoint.
- `POST /revocation` – revoke access or refresh tokens (RFC 7009).
- `POST /introspect` – token introspection (RFC 7662).
- `GET /userinfo` – return profile claims about authenticated subject.
- `GET /logout` or `/end-session` – session termination.
- `POST /register` (optional) – dynamic client registration.

## SECTION 9 — CONFIGURATION REQUIREMENTS
- RSA/ECDSA key pair for signing; private key secured with key vault; public key published via JWKS.
- Support for key rotation and multiple keys active (kid headers).
- Settings for token lifetimes (access, refresh, id tokens) per client or global.
- Database connection strings for clients/authorizations.
- Options for supported grant types and CORS origins.
- Rate limit thresholds.
- Logging settings for auditing.
- External provider credentials for social logins.

## SECTION 10 — BUILD VS BUY DECISION
**Extend this service**: offers full control, reuse existing identity and business logic, capable of customizing flows. Significant engineering effort required to implement protocol correctly and securely. Long-term maintenance burden; risk of subtle security flaws. Best if product requires tightly integrated custom features.

**Integrate IdentityServer/OpenIddict**: these libraries are mature, spec-compliant, open source, and support extensibility. They handle endpoints, token management, client stores, and security hardening. Using them reduces risk and development time. They can be hosted within the same ASP.NET Core application and integrated with existing Identity. Minor adaptation of data models may be required.

**Use a hosted provider (Auth0, Azure AD)**: fastest to market, offloads security and scaling, offers enterprise features (PKCE, MFA, GDPR compliance). But you lose full control, may incur cost, and need to integrate via federation. Suitable when timeline or compliance don't allow in-house development.

**Recommendation**: unless there is a compelling reason to build proprietary auth logic, adopt an existing framework such as IdentityServer/ OpenIddict and adapt your identity models. For enterprises with many clients and high security requirements, a hosted solution could be evaluated in parallel.

---

*End of assessment.*
