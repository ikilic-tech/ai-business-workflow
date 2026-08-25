# AI-Native Engineering: A Case Study

## Abstract

This document presents a case study of building a production-grade .NET 8 Web API using AI-native engineering methodology. The project demonstrates that AI can participate meaningfully in the full software development lifecycle — from architecture through implementation, testing, security, and documentation — while humans retain responsibility for direction, review, and final decisions.

## 1. Background

### Problem Statement

Enterprise software generates vast amounts of operational data — customer records, sales activities, visit notes, opportunities — but often fails to convert this data into timely, actionable decisions.

### Hypothesis

AI can address this problem at two levels:

1. **Runtime capability**: AI as an intelligence layer that analyzes business data and produces structured insights.
2. **Engineering capability**: AI as a development partner that participates in planning, implementation, testing, and documentation.

## 2. Methodology

### Development Process

The project was developed using Claude Code as the primary AI engineering environment. The workflow followed a consistent pattern:

```
Human defines goal → AI explores options → Human decides approach →
AI implements → Automated tests verify → AI reviews → Human approves
```

### Key Principles

- **AI output is untrusted**: All AI-generated analysis goes through structured validation (score clamping, enum normalization, null safety).
- **Defense in depth**: Prompt injection protection uses sanitization, data boundary tags, and output validation.
- **Measurability**: Every AI call is instrumented with timing metrics.
- **Deterministic baseline**: A rule-based service provides comparison for AI output quality.

## 3. Architecture

### System Architecture

```
Client → Middleware Pipeline → API Layer → Service Layer → IAiService → OpenAI → Validation → Response
```

### AI Provider Abstraction

The `IAiService` interface decouples business logic from the AI provider. Three implementations exist:

| Implementation | Purpose |
|---|---|
| `AiService` | Production — calls OpenAI Responses API |
| `MeteredAiService` | Decorator — adds timing/metrics to any IAiService |
| `DeterministicBaselineService` | Baseline — rule-based analysis without AI |
| `FakeAiService` | Testing — deterministic responses for integration tests |

### Prompt Engineering

Prompts are extracted into versioned classes (`Prompts/`) with:
- Semantic version (e.g., v1.0.0)
- Purpose and expected I/O documentation
- `InputSanitizer` applied to all user-provided data
- `<user_data>` boundary tags separating data from instructions

## 4. Security Architecture

### Threat Model

AI-specific threats addressed:

| Threat | Mitigation |
|---|---|
| Prompt injection | InputSanitizer + `<user_data>` boundary tags |
| Data boundary escape | `</user_data>` tag sanitization |
| Template injection | `{{` and `}}` breaking |
| Code fence injection | ` ``` ` breaking |
| Unsafe AI output | AiResponseValidator (score clamping, enum normalization) |
| Timing attacks | `CryptographicOperations.FixedTimeEquals` for API keys |

### Adversarial Evaluation

13 adversarial tests cover attack vectors including:
- XML boundary escape
- Role-playing attacks
- System prompt extraction
- JSON payload injection
- Combined multi-vector attacks

## 5. Testing Strategy

### Test Distribution

| Category | Count | Purpose |
|---|---|---|
| Model validation | ~40 | DataAnnotations, required fields, ranges |
| Service unit tests | ~60 | AiService parsing, validation, sanitization |
| Controller tests | ~20 | Request handling, error responses |
| Integration tests | ~30 | Full HTTP pipeline with FakeAiService |
| Adversarial tests | 13 | Prompt injection defense |
| Evaluation tests | 16 | Dataset validation + benchmark harness |
| Metrics tests | 6 | AiCallMetrics tracking |
| Baseline tests | 9 | DeterministicBaselineService |
| **Total** | **271** | |

### Evaluation Framework

Synthetic datasets cover all 5 analysis types with expected behaviour criteria:
- Score ranges (e.g., risk score 0-30 for low-risk customers)
- Required field presence
- Enum value validation
- Collection size expectations

## 6. Observations

### What Worked

1. **Rapid iteration**: AI-assisted implementation reduced the feedback loop between design and working code.
2. **Comprehensive testing**: AI generated tests covering edge cases that might be overlooked in manual development.
3. **Security awareness**: AI identified and mitigated security concerns (prompt injection, timing attacks) early.
4. **Documentation consistency**: AI maintained documentation in sync with implementation.

### What Required Human Judgment

1. **Architecture decisions**: Interface design, middleware ordering, deployment strategy.
2. **Security priorities**: Which threats to address, acceptable risk levels.
3. **Quality standards**: When code was "good enough" vs. needs improvement.
4. **Business context**: Understanding what business intelligence outputs are actually useful.

### Limitations

1. **No live AI benchmarks**: Evaluation datasets exist but systematic latency/quality benchmarks against live AI providers have not been run.
2. **Single developer context**: The AI-native workflow has been tested with one developer; multi-developer dynamics are unexplored.
3. **No production traffic**: The system has not been validated under real production load.

## 7. Metrics

### Codebase

| Metric | Value |
|---|---|
| Total tests | 271 |
| Test pass rate | 100% |
| API endpoints | 14 |
| Prompt versions | 5 (all v1.0.0) |
| ADRs | 4 |
| Evaluation scenarios | 9 + 10 adversarial |
| Security checks in CI | 2 (vulnerability scan + secret detection) |

### Development Phases

| Phase | Focus | Status |
|---|---|---|
| 1. Foundation | Project structure, models, health check | Complete |
| 2. AI Integration | OpenAI API, structured output, validation | Complete |
| 3. Production Readiness | Auth, CI/CD, Docker, error handling | Complete |
| 4. Business Intelligence | 5 analysis types + dashboard | Complete |
| 5. AI-Native Engineering | Evaluation, prompts, security, metrics | Complete |
| 6. Open Source Maturity | Docs, templates, examples, release policy | Complete |

## 8. Conclusion

AI-native engineering is not about replacing human developers. It is about changing the distribution of effort: humans focus on direction, architecture, and judgment while AI handles exploration, implementation, and repetitive verification.

The project demonstrates that this model can produce a well-architected, tested, and documented system — but the human role remains essential for quality, coherence, and strategic decisions.

## References

- [AI_NATIVE.md](../AI_NATIVE.md) — AI-native methodology documentation
- [ARCHITECTURE.md](../ARCHITECTURE.md) — System architecture
- [SECURITY.md](../SECURITY.md) — Security documentation
- [EVALUATION.md](../EVALUATION.md) — Evaluation framework
- [docs/adr/](adr/) — Architecture Decision Records
