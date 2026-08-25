# Architecture

## Current architecture

```text
                         Client
                           |
                           v
                 ASP.NET Core Web API
                           |
             +-------------+-------------+
             |                           |
             v                           v
     Intelligence Services          AI Service
             |                           |
             |                           v
             |                   OpenAI Responses API
             |                           |
             +-------------+-------------+
                           |
                           v
                  Structured Output Models
                           |
          +----------------+----------------+
          |                |                |
          v                v                v
     Customer Risk   Opportunity      Activity Summary
                       Analysis
          |                |                |
          +----------------+----------------+
                           |
                           v
                 Recommended Actions
```

## Layers

### Middleware

- Correlation ID
- Exception handling
- API-key authentication

### API

Controllers expose business-intelligence operations.

Minimal API endpoint extensions expose infrastructure and sample operations.

### Service

`IAiService` provides the abstraction between business logic and AI provider implementation.

### Provider

Current implementation uses the OpenAI Responses API.

### Structured output

AI results are mapped into typed application models.

## Architectural principles

### Provider independence

Business logic must not depend directly on an AI SDK.

```text
Business Logic
      |
      v
  IAIService
      |
      +---- OpenAI
      +---- Azure OpenAI
      +---- Other provider
```

Only provider-specific implementation should change when another provider is introduced.

### Structured output

```text
AI
 ↓
Structured response
 ↓
Validation
 ↓
Application
```

Free-form AI output should not directly drive business-critical behaviour.

### Parallel analysis

The dashboard executes independent analyses concurrently where possible.

This is intended to reduce total response time compared with sequential execution.

The actual improvement must be measured by the evaluation suite.

## Future architecture

Potential future components:

- persistence layer
- evaluation dataset store
- prompt/version registry
- model routing
- observability
- asynchronous jobs
- human feedback loop
- model comparison
- cost tracking

Each component should be added only when justified by a measurable requirement.

## Architecture decisions

See `docs/adr/`.

- ADR-001: AI service abstraction
- ADR-002: Structured AI output
- ADR-003: Prompt-injection protection
- ADR-004: Parallel intelligence analysis
