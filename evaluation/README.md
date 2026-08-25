# Evaluation Framework

## Purpose

Replace qualitative claims about AI output quality with reproducible measurements.

## Structure

```text
evaluation/
  datasets/          Synthetic input data with expected behaviour criteria
  scenarios/         Evaluation scenario definitions
  results/           Evaluation run outputs (not committed until produced)
```

## Datasets

| File | Analysis Type | Scenarios |
|---|---|---|
| `customers.json` | Customer risk assessment | 3 (low/medium/high risk) |
| `opportunities.json` | Opportunity analysis | 2 (strong/at-risk) |
| `activities.json` | Activity summary | 1 (mixed activity types) |
| `business-processes.json` | Business process analysis | 2 (manual/optimised) |
| `recommended-actions.json` | Recommended actions | 1 (declining performance) |

Each dataset entry contains:
- `id`: unique scenario identifier
- `description`: what the scenario tests
- `input`: the data to send to the API
- `expectedBehaviour`: validation criteria for the AI response

## Expected Behaviour Criteria

Expected behaviour fields do not define exact expected outputs. They define **ranges and constraints** that a reasonable AI response should satisfy:

- `riskScoreRange`: `[min, max]` — score should fall within this range
- `requiredFields`: fields that must be present and non-null
- `verdictOptions`: acceptable enum values for the verdict
- `minCount` fields: minimum number of items in a collection

## Running Evaluations

### Offline (unit tests)

The AI regression tests in `AiBusinessWorkflow.Tests` use the evaluation dataset format to test parse methods, validation, and edge cases without calling a live AI provider.

### Live (requires API key)

Live evaluation against the actual API is a separate, opt-in process:

```bash
# Start the API
cd AiBusinessWorkflow.Api
dotnet run

# Run evaluation scenarios against the live API
# (evaluation runner to be implemented)
```

Live evaluation results should record:
- commit SHA
- model used
- timestamp
- latency per request
- pass/fail per scenario
- any parse failures

## Results

The `results/` directory stores evaluation run outputs.

Results are **not committed until they are produced by a reproducible experiment**.

No benchmark numbers appear in documentation until they come from this framework.

## Adding Scenarios

1. Add a new entry to the appropriate dataset JSON file
2. Include `id`, `description`, `input`, and `expectedBehaviour`
3. Use synthetic data only — no real customer information
4. Add corresponding regression tests where appropriate
