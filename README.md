# AI Business Workflow

An AI-powered .NET 8 Web API that transforms raw business data into actionable insights — risk detection, bottleneck identification, and concrete next steps — so teams can act on their data instead of just collecting it.

## Overview

Most business applications excel at collecting data: customer records, sales activities, visit notes, opportunities, follow-up dates. What they lack is the ability to tell you what to *do* with it.

A sales manager with hundreds of customer activities in the CRM still ends up manually scanning records to figure out which accounts need attention, which deals are stalling, or where follow-ups have gone cold. The data exists; the insight doesn't.

**AI Business Workflow** bridges that gap. It accepts structured business data through a clean REST API, runs it through an AI analysis pipeline, and returns structured, actionable output: efficiency analysis, bottleneck identification, optimization recommendations, and automation opportunities.

```
Business Data → Validation & Preparation → AI Analysis → Structured Insights → Recommended Actions
```

The core principle: AI output should drive decisions and actions, not generate walls of text.

**Target audience:** Engineering teams and business stakeholders in enterprise environments who need to extract value from operational data without building an in-house ML pipeline.

## Key Features

**Currently implemented:**
- Business process analysis via OpenAI's Responses API
- Structured output — efficiency ratings, bottleneck detection, optimization recommendations, automation opportunities
- Configurable model selection (GPT-4o, GPT-5.2, or any model supported by the OpenAI SDK)
- Abstracted AI provider layer (`IAiService` interface) — swap providers without touching business logic
- Health check and AI connection test endpoints
- Interactive API documentation via Swagger UI

**Planned:**
- Customer risk scoring based on activity patterns
- Activity summarization for management reporting
- Opportunity win/loss analysis
- Structured JSON output schemas for downstream system integration
- Authentication, Docker containerization, and CI/CD pipeline

## Architecture

The project follows a layered architecture with a clear separation between API surface, business logic, and AI provider integration. The key architectural decision is the abstraction of the AI provider behind an interface, making the system provider-agnostic.

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
│  Swappable: Azure OpenAI, Anthropic, local models —          │
│  implement IAiService, register in DI, done.                 │
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

- **Interface-based AI abstraction:** The `IAiService` interface decouples business logic from the AI provider. Switching from OpenAI to Azure OpenAI or Anthropic requires a new implementation class and a single DI registration change — zero modifications to controllers or business logic.
- **Scoped DI registration:** Each HTTP request gets its own service instance, ensuring thread safety and clean lifecycle management.
- **Minimal API for infrastructure endpoints:** Health check and AI test endpoints use ASP.NET Core Minimal APIs for low overhead; business endpoints use controllers for richer routing and model binding.

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

Edit `appsettings.Local.json` and set your OpenAI API key:

```json
{
  "AI": {
    "ApiKey": "sk-your-api-key-here"
  }
}
```

> **Security note:** `appsettings.Local.json` is loaded at runtime but excluded from version control via `.gitignore`. Never commit API keys to the repository.

You can also configure the AI model:

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

The API will be available at `http://localhost:5221`. In development mode, Swagger UI is accessible at `http://localhost:5221/swagger`.

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

Analyzes a business process using AI and returns structured optimization suggestions.

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
| .NET | 8.0 (LTS) | Runtime and Web API framework | Long-term support, strong performance for API workloads, mature ecosystem |
| C# | 12 | Primary language | Modern language features (primary constructors, collection expressions), type safety |
| OpenAI SDK | 2.13.0 | AI integration | Official SDK with Responses API support — cleaner interface than Chat Completions |
| Swagger / Swashbuckle | 6.6.2 | Interactive API documentation | Industry-standard API exploration and testing |
| ASP.NET Core | 8.0 | Web framework | Built-in DI, middleware pipeline, minimal API support |

### Technology decisions

- **OpenAI Responses API over Chat Completions:** The newer Responses API (`ResponsesClient`) provides a cleaner interface and better support for structured outputs, which is essential for producing machine-readable analysis results rather than freeform text.
- **.NET 8 over .NET 9:** Deliberate choice of the LTS release for production stability. The project prioritizes reliability over cutting-edge framework features.
- **No ORM or database (yet):** The current phase focuses on the AI analysis pipeline. Persistence will be introduced when the business intelligence features (Phase 4) require historical data storage.

## Design Principles

- **AI should be useful, not impressive.** If the output doesn't help someone make a decision or take an action, it's not doing its job. The system produces structured analysis with risk levels and action items, not paragraphs of generated prose.
- **Humans stay in the loop.** The system suggests — people decide. AI augments human judgment; it doesn't replace it.
- **Structured over unstructured.** JSON with risk levels, priority rankings, and concrete action items is more valuable than freeform text. Structured output also enables downstream integration with dashboards, alerting systems, and workflow tools.
- **Start small, iterate.** Get one workflow working end-to-end before adding the next. Each phase builds on proven foundations.
- **No real data.** Everything in this repository uses synthetic/demo data. No real customer or company information is stored, processed, or committed.

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
- [x] Handle AI failures and timeouts gracefully
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

- [ ] Customer risk scoring based on activity patterns
- [ ] Activity summarization for management reports
- [ ] Opportunity win/loss analysis
- [ ] Recommended next actions engine
- [ ] Management dashboard endpoints

## Motivation

This project grows out of a pattern observed across two decades of building enterprise software — CRM systems, ERP platforms, SaaS products, and mobile applications. The pattern is consistent: organizations invest heavily in data collection but underinvest in data interpretation. The CRM has thousands of customer records; the sales team still relies on intuition and spreadsheets to decide who to call next.

AI changes this equation. Not by replacing human judgment, but by processing volume that humans can't: scanning 500 customer records to surface the 10 that need attention this week, identifying stalled deals before they're lost, flagging follow-ups that have gone cold.

This project is a concrete implementation of that vision: a clean, production-oriented API that takes business data in and returns structured, actionable analysis back. It's also an exploration of how modern AI APIs integrate into a real .NET stack — beyond chatbot demos, toward genuine business utility.

The goal is to demonstrate that AI in enterprise software doesn't have to be a black box or a science project. With the right architecture, it can be a well-tested, provider-agnostic service layer that development teams can integrate, extend, and maintain like any other part of their system.

## Contributing

Contributions are welcome. Please open an issue first to discuss what you'd like to change — it helps align expectations before investing time in implementation.

- **Bugs:** Include steps to reproduce, expected vs. actual behavior, and environment details.
- **Features:** Describe the use case and why it would be valuable.
- **Pull requests:** Keep them focused on a single concern. Follow the existing code style and conventions.

## License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 Ibrahim Kilic
