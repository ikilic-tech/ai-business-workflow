# Security

## Security philosophy

AI applications introduce conventional application risks plus AI-specific risks.

The project therefore treats security as part of architecture, development and evaluation.

## Current controls

The current implementation includes:

- prompt-injection input protection
- collection-size limits
- request-body size cap
- timing-safe API-key comparison
- API-key enforcement outside Development
- security response headers
- dashboard timeout handling
- correlation ID validation
- global exception handling
- no real customer data in the public repository

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

## Security verification roadmap

Planned:

- automated dependency vulnerability scanning
- secret scanning in CI
- static analysis
- AI-specific security regression tests
- adversarial prompt test dataset
- threat-model review per major feature
- security benchmark reporting

## Security claims

Security claims should be tied to:

- a test
- a scan
- a documented review
- or a reproducible configuration

Avoid unsupported statements such as "100% secure" or "fully OWASP compliant."
