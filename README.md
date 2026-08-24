# AI Business Workflow

A .NET 8 Web API that uses AI to analyze business data and turn it into actionable insights — risk detection, follow-up priorities, and recommended next steps.

## Overview

Most business applications are good at collecting data — customer records, sales activities, visit notes, opportunities, follow-up dates. But they're not great at telling you what to *do* with it.

A sales manager might have hundreds of customer activities in the system and still spend time manually figuring out which accounts need attention, which deals are at risk, or where follow-ups have gone cold.

This project is an attempt to close that gap. It takes structured business data, runs it through an AI analysis layer, and returns practical output: risk levels, key observations, and concrete next steps.

```
Business Data → Validation & Preparation → AI Analysis → Structured Insights → Recommended Actions
```

The idea is that AI should produce something you can actually act on, not just a wall of generated text.

## Features

**What's working now:**
- Business process analysis through OpenAI
- Structured responses — risk levels, observations, recommended actions
- Swagger UI for testing endpoints
- Configurable model selection (GPT-4o, GPT-5.2, etc.)
- Health check and AI connection test endpoints
- AI layer is behind an interface, so swapping providers is straightforward

**What's coming:**
- Customer risk scoring based on activity patterns
- Activity summarization for management reports
- Opportunity analysis
- Structured JSON schemas for integration with other systems
- Auth, Docker, CI/CD

## Architecture

Pretty standard layered setup, but the key point is that the AI provider is abstracted behind an interface:

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

The `IAiService` interface means you can swap OpenAI for Azure OpenAI, Anthropic, or a local model by changing just the service implementation. Controllers and business logic don't need to know or care which provider is behind it.

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

.NET 8 is an LTS release with solid performance for API workloads. The OpenAI SDK's newer Responses API (`ResponsesClient`) was chosen over the older Chat Completions API because it has a cleaner interface and better support for structured outputs.

## Design Principles

A few things I try to stick to in this project:

- **AI should be useful, not impressive.** If the output doesn't help someone make a decision or take an action, it's not doing its job.
- **Humans stay in the loop.** The system suggests — people decide.
- **Structured over unstructured.** JSON with risk levels and action items beats a paragraph of generated text. It's also much easier to integrate downstream.
- **Start small.** Get one workflow working properly before adding the next one.
- **No real data.** Everything in this repo uses synthetic/demo data. No real customer or company information.

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

- [ ] Authentication and API key management
- [ ] Structured logging with correlation IDs
- [ ] Health monitoring and metrics
- [ ] Docker support
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Better error handling (RFC 7807 problem details)

### Phase 4 — Business Intelligence

- [ ] Customer risk scoring
- [ ] Activity summarization
- [ ] Opportunity win/loss analysis
- [ ] Recommended next actions engine
- [ ] Management dashboard endpoints

## Why This Project

I've been building enterprise software for over 20 years — everything from large-scale business systems to SaaS products and mobile apps. One thing I've noticed throughout is that companies are usually pretty good at collecting data, but not nearly as good at doing something useful with it.

AI is genuinely changing that. Not in a "replace everyone" way, but in a "process 500 customer records and tell me which 10 need attention this week" way. That's the kind of problem this project is trying to solve.

I wanted to build something concrete rather than just talk about the idea, so here it is: a clean API that takes business data in and gives actionable analysis back. It's also a good way for me to explore how these AI APIs actually work in a real .NET stack, beyond the usual chatbot demos.

## Contributing

Contributions are welcome. Please open an issue first to discuss what you'd like to change — it saves everyone time.

For bugs, include steps to reproduce. For features, describe the use case. PRs should be focused and follow the existing code style.

## License

This project is licensed under the [MIT License](LICENSE).

Copyright (c) 2026 İbrahim Kılıç