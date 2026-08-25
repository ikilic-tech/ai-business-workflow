# Cost, Quality & Latency Analysis

## Overview

This document provides a framework for measuring and comparing the cost, quality, and latency trade-offs of different AI provider configurations used in the AI Business Workflow API.

## Measurement Infrastructure

### Latency Tracking

The `AiCallMetrics` system records per-operation timing automatically via the `MeteredAiService` decorator.

| Metric | Source | Description |
|---|---|---|
| Call count | `AiCallMetrics` | Total calls per operation type |
| Average latency | `AiCallMetrics` | Mean response time (ms) |
| P95 latency | `AiCallMetrics` | 95th percentile response time (ms) |
| Min / Max latency | `AiCallMetrics` | Range of observed response times |
| Success rate | `AiCallMetrics` | Percentage of non-error responses |

Access live metrics at `GET /api/ai/metrics`.

### Quality Measurement

Quality is measured using the evaluation framework (`evaluation/datasets/`):

| Dimension | Method |
|---|---|
| Structural correctness | Response matches typed model (HTTP 200, valid JSON, required fields) |
| Score accuracy | Scores fall within expected ranges per scenario |
| Directional correctness | High-risk scenarios score higher than low-risk scenarios |
| Enum validity | Risk levels, verdicts, trends match valid enum values |
| Completeness | Collections (risks, recommendations, actions) are non-empty |
| Adversarial robustness | 13 prompt injection tests pass without leakage |

### Cost Estimation

Cost depends on the AI provider and model. For OpenAI:

| Model | Input (per 1M tokens) | Output (per 1M tokens) | Notes |
|---|---|---|---|
| GPT-4o | $2.50 | $10.00 | Recommended for production |
| GPT-4o-mini | $0.15 | $0.60 | Budget option |
| GPT-4.1 | $2.00 | $8.00 | Newer model variant |

> Token pricing may change. Verify current rates at [OpenAI Pricing](https://openai.com/pricing).

## Baseline Comparison

The `DeterministicBaselineService` provides a zero-cost, zero-latency baseline:

| Metric | Deterministic Baseline | AI Provider (Expected) |
|---|---|---|
| Cost per call | $0.00 | ~$0.01-0.05 |
| Latency | <1 ms | 500-3000 ms |
| Structural correctness | 100% | 100% (with validation) |
| Directional accuracy | High (keyword heuristics) | Higher (contextual understanding) |
| Nuance / specificity | Low (generic text) | High (tailored recommendations) |
| Adversarial robustness | N/A (no prompt) | Dependent on sanitization |

The baseline demonstrates that simple heuristics can produce directionally correct results. The value of AI lies in nuance, specificity, and actionability beyond what rules can achieve.

## Per-Analysis Cost Estimate

Estimated token usage per analysis type (approximate):

| Analysis Type | Input Tokens | Output Tokens | GPT-4o Cost | GPT-4o-mini Cost |
|---|---|---|---|---|
| Customer Risk | ~800 | ~400 | ~$0.006 | ~$0.0004 |
| Opportunity Analysis | ~700 | ~350 | ~$0.005 | ~$0.0003 |
| Activity Summary | ~600 | ~300 | ~$0.005 | ~$0.0003 |
| Business Process | ~500 | ~400 | ~$0.005 | ~$0.0003 |
| Recommended Actions | ~600 | ~500 | ~$0.007 | ~$0.0004 |
| **Dashboard (all 5)** | **~3200** | **~1950** | **~$0.028** | **~$0.0017** |

> These are estimates based on prompt structure and expected output size. Actual usage varies with input data length.

## Latency Targets

| Tier | Target | Use Case |
|---|---|---|
| Interactive | P95 < 3s | Dashboard, single analysis |
| Batch | P95 < 10s | Bulk customer scoring |
| Background | No SLA | Nightly reports, trend analysis |

## Quality vs Cost Trade-offs

| Scenario | Recommendation | Rationale |
|---|---|---|
| Real-time dashboard | GPT-4o-mini | Speed matters, acceptable quality |
| Critical risk assessment | GPT-4o | Accuracy justifies higher cost |
| Bulk scoring (1000+ customers) | GPT-4o-mini or Baseline | Cost control at scale |
| Development / testing | FakeAiService | Zero cost, deterministic |
| Quality benchmarking | Both models | Compare outputs on same scenarios |

## Running Cost Analysis

To measure actual costs against a live provider:

1. Configure API key in `appsettings.json`
2. Run evaluation datasets through the API
3. Check `GET /api/ai/metrics` for latency data
4. Calculate cost from OpenAI usage dashboard

```bash
# Run evaluation scenarios
./examples/api-examples.sh

# Check metrics
curl http://localhost:5221/api/ai/metrics | jq .
```

## Monitoring Recommendations

For production deployment:

1. **Alert on P95 latency > 5s** — indicates provider degradation
2. **Alert on success rate < 95%** — indicates API issues
3. **Track daily cost** — set budget alerts on provider dashboard
4. **Compare model outputs monthly** — ensure quality hasn't degraded
5. **Review metrics endpoint weekly** — identify operation-specific trends

## Next Steps

1. Run live benchmarks with both GPT-4o and GPT-4o-mini
2. Record actual token usage per analysis type
3. Establish quality thresholds (minimum acceptable score accuracy)
4. Add token counting to `AiCallMetrics` (requires provider SDK support)
5. Build cost dashboard aggregating metrics over time
