# AI Evaluation & Benchmarking

## Goal

The evaluation framework exists to replace qualitative claims with reproducible measurements.

The project should measure four dimensions:

1. AI output quality
2. engineering/runtime performance
3. reliability
4. cost

## Evaluation dataset

Create a versioned synthetic dataset containing representative scenarios:

```text
evaluation/
  datasets/
    customers.json
    opportunities.json
    activities.json
    business-processes.json
```

The dataset must contain expected evaluation criteria without containing private customer information.

## Evaluation categories

### Customer risk

Evaluate:

- risk classification
- risk-factor relevance
- recommendation usefulness
- consistency

### Opportunity analysis

Evaluate:

- win-probability calibration
- strength/weakness relevance
- competitive-position accuracy
- next-step usefulness

### Activity summary

Evaluate:

- numerical consistency
- trend detection
- category aggregation
- summary relevance

### Recommended actions

Evaluate:

- priority correctness
- action relevance
- impact/effort consistency
- goal alignment

## Baseline comparison

Where meaningful, compare AI output against a deterministic baseline.

Example:

```text
Historical/business rules
        vs
AI analysis
```

Metrics may include:

- precision
- recall
- agreement
- calibration
- human rating
- invalid-output rate

Do not publish values until measured.

## Runtime metrics

Measure:

- p50 latency
- p95 latency
- p99 latency
- timeout rate
- retry rate
- successful request rate
- token usage
- estimated cost per analysis
- dashboard parallel vs sequential latency

## Structured-output reliability

Measure:

```text
Valid structured responses
--------------------------
Total AI responses
```

Also track:

- schema validation failures
- parse failures
- incomplete responses
- retries

## Regression testing

Every important prompt/model change should run the evaluation suite.

A change should report:

```text
Baseline
New version
Difference
Pass / Fail
```

## Model comparison

Future experiments may compare multiple supported models.

Compare:

- quality
- latency
- cost
- reliability

Do not choose a model based only on benchmark quality.

## Human evaluation

For subjective dimensions, create a blinded evaluation process.

Example:

```text
Response A
Response B
    ↓
Independent reviewer
    ↓
Quality score
    ↓
Reason
```

The evaluator should not know which response came from which model where practical.

## Reproducibility

Each benchmark run should record:

- repository commit SHA
- dataset version
- model
- configuration
- evaluation version
- timestamp
- result summary

## Publication rule

Only measured results may appear in:

- README
- technical articles
- presentations
- Global Talent evidence
- public claims

Hypotheses must be labelled as hypotheses.
