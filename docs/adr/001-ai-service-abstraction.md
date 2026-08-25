# ADR-001: AI Service Abstraction

## Status

Accepted

## Date

2026-01-15

## Context

The application depends on AI to produce business intelligence. The initial implementation targets OpenAI, but the architecture should not be locked to a single provider.

Future requirements may include:
- switching to Azure OpenAI for enterprise compliance
- using Anthropic Claude for certain analysis types
- running a local model for cost-sensitive or offline scenarios
- A/B testing between providers

## Problem

How should AI provider access be structured so that business logic remains independent of the specific provider SDK?

## Decision

Introduce an `IAiService` interface that defines the business-level AI operations. The concrete implementation (`AiService`) depends on the OpenAI SDK. Business logic (controllers, endpoints) depends only on the interface.

```text
Controllers / Endpoints
        |
        v
    IAiService (interface)
        |
        v
    AiService (OpenAI implementation)
        |
        v
    ResponsesClient (OpenAI SDK)
```

Registration uses scoped DI lifetime:

```csharp
builder.Services.AddScoped<IAiService, AiService>();
```

## Alternatives Considered

### Direct SDK usage in controllers

Simpler initially, but locks every controller to a specific provider. Testing requires mocking SDK internals.

### Strategy pattern with runtime selection

More flexible but premature. The current requirement is a single provider with the option to swap.

### HTTP-level abstraction

Wrapping AI calls behind a generic HTTP client. Loses type safety and structured output benefits.

## Consequences

### Positive

- Business logic is testable without a live AI provider
- Swapping providers requires one new class and one DI registration change
- Integration tests use `FakeAiService` for deterministic results
- Controllers don't reference any AI SDK types

### Negative

- Slight indirection for a single-provider deployment
- Interface must be updated when new AI operations are added

### Risks

- If providers have fundamentally different capabilities, the interface may need provider-specific extensions
