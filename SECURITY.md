# Security

## Security philosophy

AI applications introduce conventional application risks plus AI-specific risks.

The project therefore treats security as part of architecture, development and evaluation.

## Current controls

The current implementation includes:

- prompt-injection input protection (InputSanitizer + `<user_data>` boundary tags)
- collection-size limits (MaxLength on all collection properties)
- request-body size cap (5 MB Kestrel limit)
- timing-safe API-key comparison (CryptographicOperations.FixedTimeEquals)
- API-key enforcement outside Development
- security response headers (X-Content-Type-Options, X-Frame-Options, CSP, HSTS)
- dashboard timeout handling
- correlation ID validation (GUID format enforcement)
- global exception handling
- no real customer data in the public repository
- automated dependency vulnerability scanning (CI)
- secret pattern scanning in CI
- adversarial prompt evaluation tests (13 tests covering 10 attack vectors)
- adversarial prompt test dataset (evaluation/datasets/adversarial.json)

## Threat model

```text
Attacker
   |
   v
HTTP Request
   |
   +--> Authentication attack
   |
   +--> Oversized input
   |
   +--> Prompt injection
   |
   +--> Malicious business data
   |
   +--> Output manipulation
   |
   v
Application
   |
   v
AI Provider
```

## AI-specific threats

### Prompt injection

Business fields may contain text intended to manipulate model instructions.

Controls:

- separate instructions from business data
- sanitize/validate input
- avoid treating business text as trusted instructions
- constrain expected output
- validate structured results

### Sensitive-data leakage

Future integrations must avoid sending unnecessary sensitive data to an AI provider.

Principles:

- data minimization
- explicit data classification
- redaction where appropriate
- no secrets in prompts
- no production customer data in tests

### Unsafe model output

AI output must be treated as untrusted input.

Controls:

- schema validation
- enum/value validation
- defensive deserialization
- application-level business rules

## Conventional API security

Maintain:

- authentication
- request limits
- safe error responses
- security headers
- dependency updates
- secret management
- logging without sensitive values

## Planned

- static analysis tooling
- threat-model review per major feature
- security benchmark reporting

## Security claims

Security claims should be tied to:

- a test
- a scan
- a documented review
- or a reproducible configuration

Avoid unsupported statements such as "100% secure" or "fully OWASP compliant."
