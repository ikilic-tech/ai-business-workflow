# Benchmark Results

## Overview

This document reports the results of running the evaluation framework against the API. The benchmark harness validates response structure, field presence, score ranges, and timing.

## Test Environment

| Parameter | Value |
|---|---|
| Runtime | .NET 8.0 LTS |
| AI Provider | FakeAiService (deterministic) |
| Test Framework | xUnit + FluentAssertions |
| Evaluation Datasets | 9 scenarios across 5 analysis types |
| Adversarial Datasets | 10 attack scenarios |

> **Note:** These benchmarks run against `FakeAiService`, not a live AI provider. They validate the pipeline (routing, validation, serialization, response structure) rather than AI output quality. Live AI benchmarks should be run separately with a configured API key.

## Structural Validation Results

| Analysis Type | Scenarios | Status | Avg Response (ms) |
|---|---|---|---|
| Customer Risk | 3 | PASS | <10 |
| Opportunity Analysis | 2 | PASS | <10 |
| Activity Summary | 1 | PASS | <10 |
| Business Process | 2 | PASS | <10 |
| Recommended Actions | 1 | PASS | <10 |

All responses satisfy:
- HTTP 200 status code
- Correct JSON structure matching typed models
- Scores within 0-100 range
- Required fields present and non-empty
- Summary field populated

## Deterministic Baseline Results

The `DeterministicBaselineService` produces rule-based results for comparison:

| Scenario | Baseline Score | Baseline Verdict | Expected Range |
|---|---|---|---|
| High-value loyal customer | 15 (Low risk) | Low | 0-30 ✓ |
| Declining engagement customer | 50 (Medium risk) | Medium | 30-70 ✓ |
| At-risk customer (missed payments) | 90 (High risk) | High | 60-100 ✓ |
| Strong pipeline opportunity | 85% win | Likely Win | 50-85% ✓ |
| Stalled opportunity | 15% win | Likely Loss | 10-45% ✓ |
| Manual invoice process | 35 efficiency | Low | 30-70 ✓ |
| Digital onboarding | 75 efficiency | High | 65-95 ✓ |

The baseline produces directionally correct results using keyword heuristics, demonstrating that simple rules can approximate expected behaviour. AI providers should exceed this baseline in nuance, specificity, and actionability.

## Adversarial Evaluation Results

| Attack Vector | Scenarios | Sanitized | Tags Intact |
|---|---|---|---|
| XML boundary escape | 2 | ✓ | ✓ |
| Instruction override | 1 | ✓ | ✓ |
| Role-playing | 1 | ✓ | ✓ |
| System prompt extraction | 1 | ✓ | ✓ |
| Code fence JSON injection | 2 | ✓ | ✓ |
| Delimiter confusion | 1 | ✓ | ✓ |
| Combined multi-vector | 1 | ✓ | ✓ |
| Nested escape | 1 | ✓ | ✓ |

All 13 adversarial tests pass. Key defenses:
- `InputSanitizer` breaks `{{`, `}}`, `` ``` ``, `<user_data>`, `</user_data>`
- `<user_data>` boundary tags contain all user-provided data
- Injected content remains inside data boundaries
- Prompt structure is not compromised by any tested attack vector

## Dataset Validation Results

| Check | Result |
|---|---|
| All dataset files exist (6 files) | PASS |
| All datasets are valid JSON | PASS |
| All scenarios have required fields (id, description, input, expectedBehaviour) | PASS |
| All scenario IDs are unique across datasets | PASS |
| Customer risk score ranges are valid (0-100, min ≤ max) | PASS |
| Adversarial dataset has required fields (vector, payload, targetPrompt) | PASS |
| Validation checks have required fields (id, name, appliesTo) | PASS |

## Metrics Infrastructure

The `AiCallMetrics` system tracks per-operation:

| Metric | Description |
|---|---|
| Call count | Total calls per operation type |
| Average latency | Mean response time in ms |
| P95 latency | 95th percentile response time |
| Min/Max latency | Range of response times |
| Success rate | Percentage of successful calls |

Access via `GET /api/ai/metrics` at runtime.

## Next Steps

1. **Live AI benchmarks**: Run evaluation datasets against OpenAI with real API key
2. **Quality comparison**: Compare AI output vs deterministic baseline on the same scenarios
3. **Cost tracking**: Measure token usage per analysis type
4. **Multi-model comparison**: Compare GPT-4o vs GPT-4o-mini on quality/cost trade-offs
5. **Latency profiling**: Measure P50/P95/P99 under concurrent load
