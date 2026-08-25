# ADR-003: Prompt Injection Protection

## Status

Accepted

## Date

2026-02-10

## Context

The API accepts user-supplied business data (company names, descriptions, notes, activities) and interpolates this data into AI prompts. This creates a prompt injection attack surface where malicious input could manipulate AI behaviour.

OWASP identifies injection as a top application security risk. For LLM-powered applications, prompt injection is the AI-specific equivalent.

## Problem

How should user-supplied data be handled before being included in AI prompts to prevent prompt injection?

## Decision

Apply a defence-in-depth strategy:

### 1. Input sanitization

`InputSanitizer.Sanitize()` processes all user-supplied fields before prompt interpolation:

- Breaks `{{` into `{ {` (prevents template injection in interpolated strings)
- Breaks `}}` into `} }`
- Breaks triple backticks into separated characters (prevents code fence injection)
- Uses `while` loops to handle nested patterns

### 2. Data/instruction separation

Prompts use `<user_data>` XML tags to clearly separate user-supplied data from system instructions:

```text
Analyze the following...
<user_data>
Company: {{sanitized_value}}
</user_data>
Return ONLY a valid JSON object...
```

### 3. Output constraint

Prompts explicitly request structured JSON output with a defined schema, reducing the AI's tendency to follow injected instructions.

### 4. Identity override

Identity fields (IDs, names) are overridden server-side after deserialization, preventing AI from returning manipulated identifiers.

## Alternatives Considered

### No sanitization, rely on AI model safety

Insufficient. Models can be manipulated even with safety training.

### Allow-list only characters

Too restrictive for business data that legitimately contains special characters.

### Separate API call for sanitization

Unnecessary complexity. Static string operations are sufficient for the current threat model.

## Consequences

### Positive

- Documented, testable sanitization layer
- Clear separation of instructions and data in prompts
- Regression tests verify sanitization behaviour
- Defence-in-depth with multiple layers

### Negative

- Sanitization may alter legitimate data containing `{{` or backticks (rare in business data)
- Cannot prevent all possible prompt injection techniques

### Risks

- Novel prompt injection techniques may bypass current sanitization
- Regular review of the threat model is necessary
