# AI Business Workflow

A .NET 8 Web API that uses AI to analyze business data and produce structured intelligence: customer risk scoring, opportunity analysis, activity summarization, workflow optimization, and recommended actions.

The project also explores an AI-native engineering approach where AI participates in planning, implementation, testing, security review, and documentation — while humans retain responsibility for architecture, verification, and decisions.

## Overview

Most business applications collect operational data — customer records, sales activities, visit notes, opportunities — but often fail to convert this data into timely, actionable decisions.

This API takes structured business data, runs it through an AI analysis layer, and returns typed intelligence that can inform human decisions.

```
Business Data → Validation → AI Analysis → Structured Output → Validation → Response
```

## Architecture

```
Client
  │
  ▼
ASP.NET Core (Middleware: Correlation ID, Auth, Error Handling)
  │
  ├── Controllers / Minimal API Endpoints
  │
  ▼
IAiService (interface)
  │
  ├── AiService (OpenAI Responses API)
  ├── MeteredAiService (timing decorator)
  ├── DeterministicBaselineService (rule-based comparison)
  └── FakeAiService (deterministic testing)
  │
  ▼
Structured Output Models → AiResponseValidator → Response
```

See [ARCHITECTURE.md](ARCHITECTURE.md) and [docs/adr/](docs/adr/) for architecture decision records.

## AI Integration

The AI layer is designed around provider independence and output safety:

- **Provider abstraction** — `IAiService` interface decouples business logic from OpenAI. Switching providers requires only a new implementation.
- **Structured output** — All AI responses are deserialized into typed C# models, not free-form text.
- **Output validation** — `AiResponseValidator` clamps scores to valid ranges, normalizes enum values, and initializes null collections.
- **Prompt injection protection** — `InputSanitizer` breaks dangerous patterns (`{{`, `}}`, code fences, boundary tags). All user data is enclosed in `<user_data>` boundary tags.
- **Metrics** — `MeteredAiService` decorator tracks call count, latency (avg/p95/min/max), and success rate per operation via `AiCallMetrics`.
- **Deterministic baseline** — `DeterministicBaselineService` provides rule-based analysis for quality comparison against AI output.
- **Versioned prompts** — All prompt templates are extracted into versioned classes in `Prompts/` with documented purpose and expected I/O.

## Business Intelligence

| Endpoint | Description |
|---|---|
| `POST /api/business-workflow/analyze` | Process efficiency, bottlenecks, recommendations, automation opportunities |
| `POST /api/intelligence/customer-risk` | Churn risk score, risk factors, engagement trend |
| `POST /api/intelligence/activity-summary` | Activity volume, trends, categories, key findings |
| `POST /api/intelligence/opportunity-analysis` | Win probability, strengths/weaknesses, competitive position |
| `POST /api/intelligence/recommended-actions` | Prioritized actions with impact/effort ratings |
| `POST /api/intelligence/dashboard` | Runs multiple analyses in parallel, returns combined result |

## Evaluation

The project includes an evaluation framework for measuring AI output quality:

- **Synthetic datasets** — 9 scenarios across 5 analysis types with expected behaviour criteria (score ranges, required fields, enum values)
- **Adversarial tests** — 13 tests covering 10 attack vectors (XML boundary escape, instruction override, role-playing, JSON injection, multi-vector attacks)
- **Benchmark harness** — Runs evaluation datasets through the API with timing measurement and structural validation
- **Deterministic baseline** — Rule-based comparison service using keyword heuristics
- **Latency tracking** — Per-operation metrics accessible at `GET /api/ai/metrics`

Current benchmarks run against `FakeAiService` (deterministic) to validate the pipeline. Live AI benchmarks are planned.

See [EVALUATION.md](EVALUATION.md) and [docs/BENCHMARK_RESULTS.md](docs/BENCHMARK_RESULTS.md).

## Security

- Prompt injection protection (InputSanitizer + `<user_data>` boundary tags)
- Collection size limits and request body size cap (5 MB)
- Timing-safe API key comparison
- API key enforcement in non-Development environments
- Security response headers (X-Content-Type-Options, X-Frame-Options, CSP, HSTS)
- Dashboard timeout handling
- Correlation ID validation
- Dependency vulnerability scanning and secret detection in CI
- Adversarial prompt evaluation test suite

See [SECURITY.md](SECURITY.md).

## Getting Started

### Prerequisites

- .NET 8 SDK
- OpenAI API key (for live AI; not required for running tests)
- Docker (optional)

### Install and Run

```bash
git clone https://github.com/ikilic-tech/ai-business-workflow.git
cd ai-business-workflow
dotnet restore
cd AiBusinessWorkflow.Api
dotnet run
```

Swagger UI: `http://localhost:5221/swagger`

### Configuration

Create `AiBusinessWorkflow.Api/appsettings.Local.json`:

```json
{
  "AI": {
    "Provider": "OpenAI",
    "Model": "gpt-4o",
    "ApiKey": "sk-your-api-key"
  },
  "Authentication": {
    "ApiKeys": ["your-api-key"]
  }
}
```

Do not commit API keys. In Development mode, authentication is skipped when no keys are configured.

### Docker

```bash
AI_API_KEY=sk-your-key docker compose up --build
```

### Tests

```bash
dotnet test
```

Tests use `FakeAiService` and require no API keys or network access.

## API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/health` | No | Application health + AI connectivity |
| GET | `/api/ai/test` | Yes | Test AI provider connection |
| GET | `/api/ai/metrics` | Yes | Per-operation latency and success metrics |
| POST | `/api/ai/metrics/reset` | Yes | Reset metrics counters |
| POST | `/api/business-workflow/analyze` | Yes | Business process analysis |
| POST | `/api/intelligence/customer-risk` | Yes | Customer risk assessment |
| POST | `/api/intelligence/activity-summary` | Yes | Activity summarization |
| POST | `/api/intelligence/opportunity-analysis` | Yes | Opportunity analysis |
| POST | `/api/intelligence/recommended-actions` | Yes | Recommended actions |
| POST | `/api/intelligence/dashboard` | Yes | Combined dashboard (parallel) |
| GET | `/api/samples/*` | No | Synthetic sample data |

Authentication uses `X-Api-Key` header. Errors follow RFC 7807 ProblemDetails.

## Project Structure

```
AiBusinessWorkflow.Api/
├── Controllers/          # Business intelligence endpoints
├── Endpoints/            # Infrastructure + sample data (minimal API)
├── HealthChecks/         # AI connectivity + memory checks
├── Middleware/            # Correlation ID, auth, error handling
├── Models/               # Request/response types
├── Prompts/              # Versioned prompt templates (v1.0.0)
├── Services/AI/          # IAiService, AiService, MeteredAiService,
│                         # DeterministicBaselineService, InputSanitizer,
│                         # AiResponseValidator, AiCallMetrics
└── Data/                 # Sample data generators

AiBusinessWorkflow.Tests/
├── Unit/                 # Model validation, service, controller tests
├── Integration/          # Full HTTP pipeline tests (FakeAiService)
└── Evaluation/           # Dataset validation + benchmark harness

evaluation/
└── datasets/             # Synthetic evaluation scenarios (JSON)

docs/
├── adr/                  # Architecture Decision Records
├── BENCHMARK_RESULTS.md
├── CASE_STUDY.md
├── COST_QUALITY_LATENCY.md
└── LESSONS_LEARNED.md

examples/
├── api-examples.sh       # curl examples
└── python-client.py      # Python integration example
```

## Engineering Approach

This project was built using an AI-native engineering workflow with Claude Code:

**Human responsibilities:** defining objectives, making architectural decisions, reviewing implementation, validating behaviour, approving changes.

**AI assistance:** exploring alternatives, generating implementation candidates, proposing tests, debugging, identifying refactoring opportunities, drafting documentation, security review.

**Automation:** building, testing, evaluating, scanning, validating.

See [AI_NATIVE.md](AI_NATIVE.md) for the full methodology.

## Limitations

- Uses synthetic data only — no real customer information
- No persistence layer (in-memory processing)
- AI output quality depends on the model and prompt configuration
- Evaluation benchmarks run against `FakeAiService`, not a live AI provider
- Cost estimates in documentation are approximations, not measured values
- Tested with a single-developer workflow

## Future Work

See [ROADMAP.md](ROADMAP.md) for planned improvements including additional AI providers, database persistence, caching, rate limiting, and live benchmarking.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT — see [LICENSE](LICENSE).
