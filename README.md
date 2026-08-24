# AI Business Workflow

A .NET 8 Web API that uses AI to analyze business data and turn it into actionable insights: risk detection, bottleneck identification, and recommended next steps.

## Overview

Most business applications are good at collecting data. Customer records, sales activities, visit notes, opportunities, follow-up dates. But they're not great at telling you what to *do* with it.

A sales manager might have hundreds of customer activities in the system and still spend time manually figuring out which accounts need attention, which deals are at risk, or where follow-ups have gone cold. The information is all there, but nobody has time to sift through it.

This project tries to close that gap. It takes structured business data through a REST API, runs it through an AI analysis layer, and gives back something useful: efficiency analysis, bottleneck detection, optimization suggestions, and automation opportunities.

```
Business Data → Validation & Preparation → AI Analysis → Structured Insights → Recommended Actions
```

The idea is simple: if AI output doesn't help someone make a decision or take an action, it's not doing its job.

**Who is this for?** Engineering teams and business stakeholders who want to get more value out of their operational data without building a full ML pipeline from scratch.

## Key Features

**What's working now:**
- Business process analysis via OpenAI's Responses API
- Structured output: efficiency ratings, bottleneck detection, optimization recommendations, automation opportunities
- Configurable model selection (GPT-4o, GPT-5.2, or any model the OpenAI SDK supports)
- AI provider is behind an interface (`IAiService`), so swapping to a different provider doesn't touch business logic
- Health check and AI connection test endpoints
- Swagger UI for interactive API docs

**What's coming:**
- Customer risk scoring based on activity patterns
- Activity summarization for management reporting
- Opportunity win/loss analysis
- Structured JSON output schemas for integration with other systems
- Authentication, Docker, CI/CD

## Architecture

Pretty standard layered setup, but the important part is that the AI provider is abstracted behind an interface. This means the rest of the application doesn't know or care whether it's talking to OpenAI, Azure OpenAI, Anthropic, or a local model.

```
┌──────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│                                                              │
│  BusinessWorkflowController        Minimal API Endpoints     │
│  POST /api/business-workflow/      GET /api/health           │
│       analyze                      GET /api/ai/test          │
│                                                              │
│  Responsibilities:                                           │
│  • Request validation & routing                              │
│  • HTTP response mapping                                     │
│  • Swagger/OpenAPI documentation                             │
└────────────────────────────┬─────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│                      Service Layer                           │
│                                                              │
│  IAiService (interface)                                      │
│  ├── AnalyzeBusinessProcessAsync(BusinessProcess)            │
│  └── TestAiAsync()                                           │
│                                                              │
│  AiService (implementation)                                  │
│  ├── Prompt construction from structured input               │
│  ├── Response parsing and error handling                     │
│  └── Structured logging with ILogger<T>                      │
│                                                              │
│  Registered via DI (scoped lifetime)                         │
└────────────────────────────┬─────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│                   AI Provider Layer                          │
│                                                              │
│  OpenAI Responses API                                        │
│  └── ResponsesClient (SDK v2.13.0)                           │
│      • Model: configurable via appsettings (default: gpt-4o) │
│      • Auth: ApiKeyCredential from configuration             │
│      • Uses newer Responses API over Chat Completions        │
│                                                              │
│  Swappable: implement IAiService, register in DI, done.      │
└────────────────────────────┬─────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│                    Structured Output                         │
│                                                              │
│  • Process efficiency analysis                               │
│  • Bottleneck identification                                 │
│  • Optimization recommendations                              │
│  • Automation opportunities                                  │
│  • Risk levels and priority rankings (planned)               │
└──────────────────────────────────────────────────────────────┘
```

### Why this architecture?

- **Interface-based AI abstraction:** `IAiService` decouples business logic from the AI provider. Switching from OpenAI to Azure OpenAI or Anthropic means writing a new implementation class and changing one DI registration. Controllers don't change at all.
- **Scoped DI registration:** Each HTTP request gets its own service instance. Clean lifecycle, no thread-safety headaches.
- **Minimal API for infrastructure, controllers for business logic:** Health check and AI test use Minimal APIs (less ceremony). The actual business endpoint uses a controller for richer model binding and routing.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (LTS)
- An [OpenAI API key](https://platform.openai.com/api-keys)

### Installation

```bash
# Clone the repository
git clone https://github.com/ibrahimkilic/AiBusinessWorkflow.git
cd AiBusinessWorkflow

# Restore dependencies
dotnet restore
```

### Configuration

Create a local configuration file for your API key:

```bash
cp AiBusinessWorkflow.Api/appsettings.json AiBusinessWorkflow.Api/appsettings.Local.json
```

Edit `appsettings.Local.json` and add your OpenAI API key:

```json
{
  "AI": {
    "ApiKey": "sk-your-api-key-here"
  }
}
```

> `appsettings.Local.json` is loaded at runtime but excluded from version control via `.gitignore`. Don't commit API keys.

You can also change the model:

```json
{
  "AI": {
    "Provider": "OpenAI",
    "Model": "gpt-4o",
    "ApiKey": "sk-your-api-key-here"
  }
}
```

### Running the Application

```bash
cd AiBusinessWorkflow.Api
dotnet run
```

The API will be available at `http://localhost:5221`. In development mode, Swagger UI is at `http://localhost:5221/swagger`.

## API Reference

### Health Check

```
GET /api/health
```

Returns service health status.

```json
{
  "status": "ok",
  "service": "AiBusinessWorkflow.Api"
}
```

### AI Connection Test

```
GET /api/ai/test
```

Tests the OpenAI connection with a simple prompt and returns the response.

```json
{
  "status": "success",
  "response": "Hello! How can I assist you today?"
}
```

### Business Process Analysis

```
POST /api/business-workflow/analyze
```

Analyzes a business process using AI and returns optimization suggestions.

**Request body:**

```json
{
  "name": "Customer Onboarding",
  "description": "New customer registration and activation process",
  "inputData": "Customer form, ID document, email verification",
  "goal": "Reduce registration time and increase customer satisfaction"
}
```

**Response:**

```json
{
  "processId": "aa333b9f-aad7-4e82-9ca4-0a41a7f018bc",
  "analysis": "1. Process efficiency analysis... 2. Potential bottlenecks... 3. Optimization recommendations... 4. Automation opportunities..."
}
```

## Technology Stack

| Technology | Version | Purpose | Why |
|---|---|---|---|
| .NET | 8.0 (LTS) | Runtime and Web API framework | Long-term support, solid performance for API workloads |
| C# | 12 | Primary language | Modern features (primary constructors, collection expressions), strong type safety |
| OpenAI SDK | 2.13.0 | AI integration | Official SDK, supports the newer Responses API |
| Swagger / Swashbuckle | 6.6.2 | API documentation | Industry standard for interactive API testing |
| ASP.NET Core | 8.0 | Web framework | Built-in DI, middleware pipeline, minimal API support |

### Technology decisions

- **Responses API over Chat Completions:** The newer `ResponsesClient` has a cleaner interface and better structured output support. Since this project needs machine-readable results (not freeform text), that matters.
- **.NET 8 over .NET 9:** LTS release. For something heading toward production, stability wins over shiny new features.
- **No ORM or database yet:** The current phase focuses on the AI analysis pipeline. Persistence comes in Phase 4 when historical data storage is actually needed.

## Design Principles

A few things I try to stick to in this project:

- **AI should be useful, not impressive.** If the output doesn't help someone make a decision or take an action, it's not doing its job.
- **Humans stay in the loop.** The system suggests, people decide.
- **Structured over unstructured.** JSON with risk levels and action items beats a paragraph of generated text. It's also much easier to integrate with dashboards, alerts, or other systems downstream.
- **Start small.** Get one workflow working properly before adding the next one.
- **No real data.** Everything in this repo uses synthetic/demo data. No real customer or company information.

## Roadmap

### Phase 1 — Foundation *(mostly complete)*

- [x] Create project repository and solution structure
- [x] Define initial business workflow model
- [x] Document architecture
- [x] Create API with health check endpoint
- [ ] Add sample business data generator
- [ ] Add unit and integration tests

### Phase 2 — AI Integration *(in progress)*

- [x] Add AI analysis service with OpenAI Responses API
- [x] Add prompt management and construction
- [x] Handle AI failures and timeouts
- [ ] Define structured AI output schemas (JSON)
- [ ] Add input validation and sanitization

### Phase 3 — Production Readiness

- [ ] Authentication and API key management
- [ ] Structured logging with correlation IDs
- [ ] Health monitoring and metrics
- [ ] Docker support
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Error responses following RFC 7807 (Problem Details)

### Phase 4 — Business Intelligence

- [ ] Customer risk scoring
- [ ] Activity summarization
- [ ] Opportunity win/loss analysis
- [ ] Recommended next actions engine
- [ ] Management dashboard endpoints

## Motivation

I've been building enterprise software for over 20 years. CRM systems, ERP platforms, SaaS products, mobile apps. One thing I've seen consistently is that companies are usually good at collecting data, but not nearly as good at doing something useful with it.

AI is changing that. Not in a "replace everyone" way, but in a practical way: process 500 customer records and tell me which 10 need attention this week. Flag the deals that are going cold before someone notices three months later. That's the kind of problem this project is about.

I wanted to build something concrete rather than just talk about the idea, so here it is. A clean API that takes business data in and gives structured analysis back. It's also been a good way to dig into how these AI APIs actually work in a real .NET codebase, not just a chatbot tutorial.

The longer-term goal is to show that AI in enterprise software can be a normal, well-architected service layer. Not a black box, not a research project. Just another part of the system that teams can test, extend, and maintain.

## Contributing

Contributions are welcome. Please open an issue first to discuss what you'd like to change. It saves everyone time.

For bugs, include steps to reproduce. For features, describe the use case. PRs should be focused and follow the existing code style.

## License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 Ibrahim Kilic
