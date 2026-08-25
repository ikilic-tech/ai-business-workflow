# AI Business Workflow

A .NET 8 Web API that takes structured business data (customers, activities, opportunities, processes), sends it through an AI analysis layer, and returns typed, validated intelligence — not free-form text.

```
POST /api/intelligence/customer-risk
{
  "companyName": "Acme Industries",
  "industry": "Manufacturing",
  "paymentHistory": "Generally on time, one late payment last quarter",
  "activities": [...]
}

→ { "riskScore": 35, "riskLevel": "Low", "churnProbability": "Low", "riskFactors": [...] }
```

## Quick Start

```bash
git clone https://github.com/ikilic-tech/ai-business-workflow.git
cd ai-business-workflow
dotnet restore
cd AiBusinessWorkflow.Api
dotnet run
```

Open `http://localhost:5221/swagger` to explore the API.

Tests require no API key or network access:

```bash
cd ai-business-workflow
dotnet test
```

### Connecting an AI Provider

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

**Do not commit API keys.** `appsettings.Local.json` is in `.gitignore`. In Development mode, authentication is skipped when no keys are configured.

### Docker

```bash
AI_API_KEY=sk-your-key docker compose up --build
```

## What This Does

Most business applications collect customer records, sales activities, visit notes, and opportunities — but rarely convert that data into timely decisions.

This API accepts structured business data, runs it through an AI analysis layer with input sanitization and output validation, and returns typed C# models that can be consumed by any client.

**This is not a chatbot.** There is no conversation, no free-form text generation, no prompt exposed to the user. The API receives structured data, constructs a prompt internally, sends it to OpenAI, deserializes the response into typed models, validates the output (clamping scores, normalizing enums, initializing nulls), and returns a structured JSON response.

```
Client sends business data (JSON)
       │
       ▼
  Input validation (DataAnnotations)
       │
       ▼
  InputSanitizer (breaks injection patterns)
       │
       ▼
  Versioned prompt template + <user_data> boundary tags
       │
       ▼
  OpenAI Responses API
       │
       ▼
  Deserialize into typed C# model
       │
       ▼
  AiResponseValidator (clamp scores 0-100, normalize enums, init nulls)
       │
       ▼
  Structured JSON response
```

## Example: Customer Risk Assessment

**Request:**

```bash
curl -X POST http://localhost:5221/api/intelligence/customer-risk \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: your-key" \
  -d '{
    "companyName": "Acme Industries",
    "industry": "Manufacturing",
    "employeeCount": 250,
    "annualRevenue": 15000000,
    "contactName": "Jane Smith",
    "contactEmail": "jane@acme.example.com",
    "accountAge": "3 years",
    "paymentHistory": "Generally on time, one late payment last quarter",
    "activities": [
      {
        "type": "Meeting",
        "date": "2024-01-15",
        "description": "Quarterly business review",
        "outcome": "Discussed expansion plans"
      },
      {
        "type": "Call",
        "date": "2024-02-20",
        "description": "Follow-up on new pricing",
        "outcome": "Requested proposal"
      }
    ]
  }'
```

**Response:**

```json
{
  "customerId": "...",
  "companyName": "Acme Industries",
  "riskScore": 35,
  "riskLevel": "Low",
  "churnProbability": "Low",
  "engagementTrend": "Stable",
  "riskFactors": [
    {
      "factor": "Payment Consistency",
      "severity": "Low",
      "description": "Payments are generally on time",
      "impact": "Minimal risk"
    }
  ],
  "recommendedActions": ["Continue regular check-ins"],
  "summary": "Low-risk customer with stable engagement..."
}
```

Every field is typed. `riskScore` is always 0-100 (clamped by `AiResponseValidator`). `riskLevel` is always a valid enum value. Collections are never null.

See `examples/api-examples.sh` for curl examples covering all endpoints.

## Analysis Types

| Endpoint | Input | Output |
|---|---|---|
| `POST /api/intelligence/customer-risk` | Customer profile + activities | Risk score (0-100), churn probability, risk factors |
| `POST /api/intelligence/opportunity-analysis` | Deal details + competitor info | Win probability (0-100), strengths, weaknesses, strategy |
| `POST /api/intelligence/activity-summary` | Department activities over a period | Volume, trends, categories, key findings |
| `POST /api/intelligence/recommended-actions` | Business area + challenges + goals | Prioritized actions with impact/effort ratings |
| `POST /api/business-workflow/analyze` | Process description + goal | Efficiency score, bottlenecks, automation opportunities |
| `POST /api/intelligence/dashboard` | Combined request | Runs multiple analyses in parallel (60s timeout) |

## Architecture

```
Client
  │
  ▼
ASP.NET Core Middleware (Correlation ID → Auth → Error Handling)
  │
  ├── Controllers (business intelligence endpoints)
  ├── Minimal API Endpoints (health, metrics, samples)
  │
  ▼
IAiService interface
  │
  ├── AiService              → OpenAI Responses API (production)
  ├── MeteredAiService       → Timing decorator (latency, success rate)
  ├── DeterministicBaseline  → Rule-based keyword heuristics (comparison)
  └── FakeAiService          → Deterministic responses (testing)
  │
  ▼
AiResponseValidator → Typed response models → Client
```

The `IAiService` interface decouples all business logic from OpenAI. Switching providers (Azure OpenAI, Anthropic, local models) requires implementing one interface with five methods. No business logic changes.

`MeteredAiService` is a decorator that wraps any `IAiService` implementation and tracks per-operation call count, latency (avg/p95/min/max), and success rate via `AiCallMetrics`. Metrics are accessible at `GET /api/ai/metrics`.

All prompt templates are versioned classes in `Prompts/` (currently v1.0.0) with documented purpose, expected input, and expected output.

See [ARCHITECTURE.md](ARCHITECTURE.md) for details and [docs/adr/](docs/adr/) for architecture decision records.

## Security

**Input protection:**
- `InputSanitizer` breaks `{{`, `}}`, code fences, and `<user_data>`/`</user_data>` tag injection attempts
- All user data is enclosed in `<user_data>` boundary tags in prompts, separating data from instructions
- Request body size capped at 5 MB (Kestrel)
- Collection size limits enforced via `[MaxLength]` attributes

**Authentication and transport:**
- API key authentication via `X-Api-Key` header (enforced in non-Development environments)
- Timing-safe key comparison (`CryptographicOperations.FixedTimeEquals`)
- Security response headers: `X-Content-Type-Options`, `X-Frame-Options`, CSP, HSTS

**AI output validation:**
- `AiResponseValidator` clamps all scores to 0-100
- Invalid enum values normalized to defaults
- Null collections initialized to empty lists

**CI security:**
- Dependency vulnerability scanning (`dotnet list package --vulnerable`)
- Secret detection in CI pipeline

**Adversarial testing:**
- 13 adversarial tests covering 10 attack vectors: XML boundary escape, instruction override, role-playing, system prompt extraction, JSON payload injection, delimiter confusion, multi-vector attacks

See [SECURITY.md](SECURITY.md) for full documentation.

## Testing

271 tests. All pass without API keys or network access.

```bash
dotnet test
```

| Category | Coverage |
|---|---|
| Model validation | DataAnnotations, required fields, value ranges |
| Service unit tests | AI response parsing, validation, sanitization |
| Controller tests | Request handling, error responses, edge cases |
| Integration tests | Full HTTP pipeline with `FakeAiService` |
| Adversarial tests | Prompt injection defense (13 tests, 10 vectors) |
| Evaluation tests | Dataset validation + benchmark harness |
| Metrics tests | `AiCallMetrics` tracking accuracy |
| Baseline tests | `DeterministicBaselineService` correctness |

Integration tests use `FakeAiService` — a deterministic `IAiService` implementation that returns fixed responses, so tests are fast, repeatable, and require no external dependencies.

## API Reference

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/health` | No | Health check + AI connectivity |
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

Authentication: `X-Api-Key` header. Errors: RFC 7807 ProblemDetails.

## Project Structure

```
AiBusinessWorkflow.Api/
├── Controllers/          # Business intelligence endpoints
├── Endpoints/            # Health, metrics, samples (minimal API)
├── HealthChecks/         # AI connectivity + memory checks
├── Middleware/            # Correlation ID, auth, error handling
├── Models/               # Request/response types with DataAnnotations
├── Prompts/              # Versioned prompt templates (v1.0.0)
├── Services/AI/          # IAiService implementations, InputSanitizer,
│                         # AiResponseValidator, AiCallMetrics
└── Data/                 # Sample data generators

AiBusinessWorkflow.Tests/
├── Unit/                 # Model, service, controller tests
├── Integration/          # Full HTTP pipeline tests
└── Evaluation/           # Dataset validation + benchmark harness

evaluation/datasets/      # Synthetic evaluation scenarios (JSON)
docs/                     # ADRs, benchmarks, case study, lessons learned
examples/                 # curl examples + Python client
```

## Engineering Approach

This project was built using an AI-native engineering workflow. AI participates in implementation, testing, security review, and documentation. Humans define objectives, make architectural decisions, review output, and approve changes.

The distinction from "AI-assisted" development: AI is not just a code completion tool here. It participates across the full lifecycle — exploring alternatives, generating test cases, identifying security concerns, and proposing refactoring. But humans own every decision.

See [AI_NATIVE.md](AI_NATIVE.md) for the methodology.

## Limitations

- Uses synthetic data only — no real customer information
- No persistence layer (in-memory processing)
- AI output quality depends on the model and prompt configuration
- Evaluation benchmarks run against `FakeAiService`, not a live AI provider
- Cost estimates in documentation are approximations, not measured values
- Tested with a single-developer workflow

## Future Work

See [ROADMAP.md](ROADMAP.md) for planned improvements including additional AI providers, database persistence, caching, rate limiting, live benchmarking, and OpenTelemetry integration.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT — see [LICENSE](LICENSE).
