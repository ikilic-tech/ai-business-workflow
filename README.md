# AI Business Workflow

An AI-powered .NET 8 Web API that transforms raw business data into structured, actionable insights — helping teams identify risks, prioritize opportunities, and decide what to do next.

## Overview

### The Problem

Business applications are effective at collecting data: customer records, sales activities, visit notes, opportunities, tasks, and follow-up dates. Most organizations have no shortage of information.

The gap is in **understanding** that information.

A sales manager with hundreds of customer activities still needs to manually review them to answer critical questions:

- Which customers need immediate attention?
- Which opportunities are at risk?
- Where are follow-ups overdue?
- What changed recently that requires action?
- What should the team focus on this week?

These questions require pattern recognition across large volumes of operational data — exactly the kind of task where AI can add practical value.

### The Solution

AI Business Workflow provides a structured pipeline that sits between business data and decision-making:

```
Business Data → Validation & Preparation → AI Analysis → Structured Insights → Recommended Actions
```

Rather than generating generic text, the system produces **structured outputs** — risk levels, prioritized observations, and concrete next steps — that integrate naturally into existing business workflows.

### Who Is This For?

- **Enterprise teams** looking to extract actionable intelligence from operational data
- **Sales managers** who need automated risk detection and follow-up tracking
- **Technical leaders** evaluating how to integrate AI into business processes without disrupting existing systems

## Key Features

**Current:**
- Business process analysis via OpenAI integration
- Structured AI responses with risk detection, observations, and recommended actions
- RESTful API with Swagger/OpenAPI documentation
- Configurable AI model selection (supports GPT-4o, GPT-5.2, and future models)
- Health check and AI connectivity verification endpoints
- Clean separation between business logic and AI layer

**Planned:**
- Customer risk scoring based on activity patterns
- Automated activity summarization for management reporting
- Opportunity analysis with win/loss probability indicators
- Structured JSON output schemas for downstream integration
- Authentication and role-based access control
- Docker containerization and CI/CD pipeline

## Architecture

The system follows a layered architecture that keeps business logic independent from the AI provider:

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                         │
│                                                     │
│  Controllers          Minimal API Endpoints         │
│  (BusinessWorkflow)   (/health, /ai/test)           │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│                 Service Layer                        │
│                                                     │
│  IAiService (interface)                              │
│  └── AiService (implementation)                     │
│      • Prompt construction                          │
│      • Response parsing                             │
│      • Error handling & logging                     │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│              AI Provider Layer                       │
│                                                     │
│  OpenAI Responses API                               │
│  └── ResponsesClient (SDK v2.13.0)                  │
│      • Model: configurable (default: gpt-4o)        │
│      • Credential management via ApiKeyCredential   │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────┐
│              Structured Output                       │
│                                                     │
│  • Process efficiency analysis                      │
│  • Bottleneck identification                        │
│  • Optimization recommendations                    │
│  • Automation opportunities                         │
└─────────────────────────────────────────────────────┘
```

**Why this structure?**

The `IAiService` interface decouples the API layer from any specific AI provider. Swapping OpenAI for Azure OpenAI, Anthropic, or a local model requires changing only the service implementation — controllers and business logic remain untouched. This is a deliberate architectural choice for long-term maintainability in enterprise environments where vendor lock-in is a real concern.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
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

> `appsettings.Local.json` is loaded at runtime but should be excluded from version control. Never commit API keys to the repository.

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

| Technology | Version | Purpose |
|---|---|---|
| .NET | 8.0 | Runtime and Web API framework |
| OpenAI SDK | 2.13.0 | AI integration via Responses API |
| Swagger / Swashbuckle | 6.6.2 | Interactive API documentation |
| C# | 12 | Primary language with modern features |

**Why .NET 8?** Long-term support, high performance for API workloads, strong typing that reduces runtime errors in enterprise systems, and mature ecosystem for building production services.

**Why OpenAI Responses API?** The newer Responses API (via `ResponsesClient`) provides a cleaner interface compared to the legacy Chat Completions API, with built-in support for structured outputs — which aligns with this project's goal of returning actionable data rather than unstructured text.

## Design Principles

### Keep AI Useful

AI should not simply generate text. The output should help someone make a decision or take an action. Every AI response in this system is structured around concrete recommendations.

### Keep Humans in the Loop

AI-generated recommendations support people rather than replace business judgment. The system provides analysis and suggestions — the final decision remains with the team.

### Use Structured Outputs

Where possible, AI responses are returned as structured data rather than unstructured text. This makes downstream integration, filtering, and automation possible.

### Start Small, Then Iterate

The first version focuses on a single, testable workflow. Each addition is validated before introducing more complexity. This approach reduces risk and produces a system where every component has been proven in practice.

### Protect Business Data

The demonstration environment uses only synthetic data. No real customer or company data is included in this repository.

## Roadmap

### Phase 1 — Foundation *(mostly complete)*

- [x] Create project repository
- [x] Define initial workflow
- [x] Document architecture
- [x] Create initial API with health check
- [ ] Add sample business data generator
- [ ] Add unit and integration tests

### Phase 2 — AI Integration *(in progress)*

- [x] Add AI analysis service with OpenAI Responses API
- [x] Add prompt management and construction
- [x] Handle AI failures and timeouts
- [ ] Define structured AI output schemas (JSON)
- [ ] Add input validation and sanitization

### Phase 3 — Production Readiness

- [ ] Add authentication and API key management
- [ ] Add structured logging with correlation IDs
- [ ] Add health monitoring and metrics
- [ ] Add Docker support with multi-stage builds
- [ ] Set up CI/CD pipeline (GitHub Actions)
- [ ] Improve error handling with problem details (RFC 7807)

### Phase 4 — Business Intelligence

- [ ] Customer risk scoring based on activity patterns
- [ ] Automated activity summarization
- [ ] Opportunity win/loss analysis
- [ ] Recommended next actions engine
- [ ] Management dashboard API endpoints

## Motivation

This project exists at the intersection of two areas where I have deep experience: **enterprise business systems** and **practical AI integration**.

Over 20 years of building software — enterprise applications, SaaS platforms, mobile applications, and business systems — one pattern has remained consistent: organizations collect far more data than they effectively use. The tools for storing and querying data have matured significantly, but the gap between raw data and informed decision-making has not closed at the same pace.

AI changes this equation. Not by replacing human judgment, but by processing operational data at a scale and speed that manual review cannot match, and surfacing the patterns that matter.

This project is a concrete implementation of that idea: a clean, extensible pipeline that takes structured business data and returns actionable intelligence. It reflects the architectural thinking and engineering discipline that come from building production systems across multiple industries, combined with a deliberate focus on making AI practically useful rather than theoretically impressive.

## Contributing

Contributions are welcome. If you would like to contribute:

1. **Open an issue** to discuss the change before starting work
2. **Fork the repository** and create a feature branch
3. **Follow existing patterns** — the codebase favors clarity over cleverness
4. **Submit a pull request** with a clear description of what changed and why

For bugs, please include steps to reproduce. For feature requests, describe the use case and expected behavior.

## License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 İbrahim Kılıç