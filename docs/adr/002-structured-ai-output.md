# ADR-002: Structured AI Output

## Status

Accepted

## Date

2026-01-20

## Context

The application uses AI to produce business intelligence results (risk scores, activity summaries, opportunity analyses, recommended actions). These results drive UI rendering, downstream logic, and dashboard aggregation.

Free-form text output would require additional parsing, be fragile, and make testing difficult.

## Problem

How should AI output be structured so that it is machine-readable, type-safe, and testable?

## Decision

AI prompts request JSON output conforming to a defined schema. The response is deserialized into strongly-typed C# models using `System.Text.Json`.

Each analysis type has:
- a prompt that includes the expected JSON schema
- a typed output model with `[JsonPropertyName]` attributes
- a parse method that strips code fences, deserializes, and overrides identity fields

```text
AI Response (raw text)
        |
        v
    StripCodeFences
        |
        v
    JsonSerializer.Deserialize<T>
        |
        v
    Override identity fields (IDs, names)
        |
        v
    Typed result object
```

Identity fields (e.g., `customerId`, `processId`) are always overridden server-side to prevent the AI from returning incorrect identifiers.

## Alternatives Considered

### Unstructured text with regex parsing

Fragile and untestable. AI output format varies between calls.

### OpenAI function calling / structured outputs mode

Would tighten the contract but adds SDK-specific coupling. The current approach works across providers.

### XML output

More verbose, no clear advantage over JSON for this use case.

## Consequences

### Positive

- Results are type-safe and serializable
- Parse methods are unit-testable with static JSON fixtures
- Identity fields cannot be spoofed by AI output
- Code fence handling makes the parser resilient to model formatting quirks

### Negative

- JSON schema must be maintained in both prompts and C# models
- AI may occasionally produce invalid JSON requiring error handling
- Schema changes require updating both prompt and model

### Risks

- Complex nested structures may increase parse failures
- Different AI models may have varying JSON compliance rates
