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
- Structured JSON output: efficiency ratings, bottleneck detection, optimization recommendations, automation opportunities
- Customer risk scoring based on activity patterns and payment history
- Activity summarization for management reporting
- Opportunity win/loss analysis with competitive positioning
- Recommended next actions engine with prioritized action items
- Management dashboard endpoint (runs all 4 analyses in parallel)
- Configurable model selection (GPT-4o, GPT-5.2, or any model the OpenAI SDK supports)
- AI provider is behind an interface (`IAiService`), so swapping to a different provider doesn't touch business logic
- Input validation with DataAnnotations
- Sample business data generator with 6 business process scenarios plus BI-specific sample data (customers, opportunities, activities)
- Health checks with AI connectivity and memory monitoring
- API key authentication middleware
- Correlation ID tracking across requests
- Global exception handling with RFC 7807 ProblemDetails
- Docker support with multi-stage build
- CI/CD pipeline with GitHub Actions
- Swagger UI for interactive API docs
- 160+ unit and integration tests

## Architecture

Pretty standard layered setup, but the important part is that the AI provider is abstracted behind an interface. This means the rest of the application doesn't know or care whether it's talking to OpenAI, Azure OpenAI, Anthropic, or a local model.

```
┌──────────────────────────────────────────────────────────────┐
│                     Middleware Pipeline                       │
│                                                              │
│  CorrelationIdMiddleware → ExceptionHandlingMiddleware →     │
│  ApiKeyAuthMiddleware                                        │
│                                                              │
│  • X-Correlation-Id tracking                                 │
│  • RFC 7807 ProblemDetails error responses                   │
│  • X-Api-Key authentication                                  │
└────────────────────────────┬─────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│                                                              │
│  BusinessWorkflowController        Minimal API Endpoints     │
│  POST /api/business-workflow/      GET /api/health           │
│       analyze                      GET /api/ai/test          │
│                                    GET /api/samples          │
│  BusinessIntelligenceController    GET /api/samples/{index}  │
│  POST /api/intelligence/           GET /api/samples/customers│
│       customer-risk                GET /api/samples/          │
│       activity-summary                  opportunities        │
│       opportunity-analysis         GET /api/samples/          │
│       recommended-actions               activities           │
│       dashboard                    GET /api/samples/          │
│                                         actions-context      │
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
│  │   → returns BusinessProcessAnalysis (structured JSON)     │
│  ├── AssessCustomerRiskAsync(CustomerProfile)                │
│  │   → returns CustomerRiskAssessment                        │
│  ├── SummarizeActivitiesAsync(ActivitySummaryRequest)        │
│  │   → returns ActivitySummaryReport                         │
│  ├── AnalyzeOpportunityAsync(Opportunity)                    │
│  │   → returns OpportunityAnalysisResult                     │
│  ├── GenerateRecommendedActionsAsync(RecommendedActionsReq)  │
│  │   → returns RecommendedActionsReport                      │
│  └── TestAiAsync()                                           │
│                                                              │
│  AiService (implementation)                                  │
│  ├── Prompt construction with JSON schema                    │
│  ├── Response parsing and deserialization                    │
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
│  BusinessProcessAnalysis model:                              │
│  • Process efficiency analysis (score, rating, explanation)  │
│  • Bottleneck identification (area, severity, fix)           │
│  • Optimization recommendations (priority, impact, effort)   │
│  • Automation opportunities (current vs proposed)            │
│  • Overall risk level and summary                            │
│                                                              │
│  Business Intelligence models:                               │
│  • CustomerRiskAssessment (risk score, churn probability)    │
│  • ActivitySummaryReport (trends, category breakdown)        │
│  • OpportunityAnalysisResult (win probability, strategy)     │
│  • RecommendedActionsReport (prioritized actions, quick wins)│
│  • DashboardSummary (all analyses combined)                  │
└──────────────────────────────────────────────────────────────┘
```

### Why this architecture?

- **Interface-based AI abstraction:** `IAiService` decouples business logic from the AI provider. Switching from OpenAI to Azure OpenAI or Anthropic means writing a new implementation class and changing one DI registration. Controllers don't change at all.
- **Scoped DI registration:** Each HTTP request gets its own service instance. Clean lifecycle, no thread-safety headaches.
- **Middleware pipeline:** Cross-cutting concerns (correlation tracking, error handling, authentication) are separated from business logic. Each middleware has a single responsibility.
- **Minimal API for infrastructure, controllers for business logic:** Health check and AI test use Minimal APIs (less ceremony). The actual business endpoint uses a controller for richer model binding and routing.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (LTS)
- An [OpenAI API key](https://platform.openai.com/api-keys)
- (Optional) [Docker](https://www.docker.com/) for containerized deployment

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

You can also change the model and configure API key authentication:

```json
{
  "AI": {
    "Provider": "OpenAI",
    "Model": "gpt-4o",
    "ApiKey": "sk-your-api-key-here"
  },
  "Authentication": {
    "ApiKeys": ["your-api-key-here"]
  }
}
```

When `Authentication:ApiKeys` is empty or not configured, API key authentication is disabled (convenient for development).

### Running the Application

```bash
cd AiBusinessWorkflow.Api
dotnet run
```

The API will be available at `http://localhost:5221`. In development mode, Swagger UI is at `http://localhost:5221/swagger`.

### Running with Docker

```bash
# Build and run
AI_API_KEY=sk-your-key docker compose up --build

# The API will be available at http://localhost:8080
```

### Running Tests

```bash
dotnet test
```

## API Reference

### Health Check

```
GET /api/health
```

Returns detailed health status including AI connectivity and memory usage.

```json
{
  "status": "Healthy",
  "totalDuration": 245.12,
  "entries": [
    {
      "name": "ai",
      "status": "Healthy",
      "description": "AI service is reachable.",
      "data": { "provider": "OpenAI", "model": "gpt-4o" }
    },
    {
      "name": "memory",
      "status": "Healthy",
      "description": "Memory usage is normal: 42.5 MB",
      "data": { "allocatedMB": 42.5 }
    }
  ]
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

### Sample Business Data

```
GET /api/samples
GET /api/samples/{index}
```

Returns pre-built sample business processes for testing and demonstration. Includes 6 scenarios: customer onboarding, invoice processing, sales lead qualification, IT support, supply chain, and performance reviews.

### Sample Business Intelligence Data

```
GET /api/samples/customers
GET /api/samples/customers/{index}
GET /api/samples/opportunities
GET /api/samples/opportunities/{index}
GET /api/samples/activities
GET /api/samples/actions-context
```

Returns sample data for business intelligence endpoints. Includes 3 customer profiles (high-value loyal, medium engagement, at-risk churn), 2 sales opportunities (strong pipeline, at-risk deal), a department activity summary, and a recommended actions context.

### Business Process Analysis

```
POST /api/business-workflow/analyze
```

Analyzes a business process using AI and returns structured optimization insights.

**Request body:**

```json
{
  "name": "Customer Onboarding",
  "description": "New customer registration and activation process",
  "inputData": "Customer form, ID document, email verification",
  "goal": "Reduce registration time and increase customer satisfaction"
}
```

**Validation rules:**
- `name`: Required, 3-200 characters
- `description`: Required, 10-2000 characters
- `inputData`: Required, 5-5000 characters
- `goal`: Required, 5-1000 characters

**Response:**

```json
{
  "processId": "aa333b9f-aad7-4e82-9ca4-0a41a7f018bc",
  "processName": "Customer Onboarding",
  "efficiency": {
    "score": 72,
    "rating": "Medium",
    "explanation": "Process has moderate efficiency with room for improvement."
  },
  "bottlenecks": [
    {
      "area": "Manual Data Entry",
      "severity": "High",
      "description": "Data is entered manually causing delays.",
      "suggestedFix": "Implement OCR-based data extraction."
    }
  ],
  "recommendations": [...],
  "automationOpportunities": [...],
  "overallRiskLevel": "Medium",
  "summary": "The process is functional but has significant optimization opportunities."
}
```

### Customer Risk Assessment

```
POST /api/intelligence/customer-risk
```

Assesses customer churn risk based on their profile, payment history, and activity patterns.

**Request body:**

```json
{
  "companyName": "TechFlow Solutions",
  "industry": "Technology",
  "employeeCount": 450,
  "annualRevenue": 85000000,
  "contactName": "Sarah Chen",
  "contactEmail": "sarah.chen@techflow.com",
  "accountAge": "4 years",
  "paymentHistory": "Consistently on time, no missed payments",
  "activities": [
    { "type": "Meeting", "date": "2024-01-22", "description": "Quarterly business review", "outcome": "Discussed expansion" }
  ]
}
```

**Response:**

```json
{
  "customerId": "...",
  "companyName": "TechFlow Solutions",
  "riskScore": 25,
  "riskLevel": "Low",
  "churnProbability": "Low",
  "engagementTrend": "Increasing",
  "riskFactors": [...],
  "recommendedActions": ["Continue regular check-ins"],
  "summary": "Low-risk customer with strong engagement."
}
```

### Activity Summary

```
POST /api/intelligence/activity-summary
```

Summarizes department activities for a given period with trends and category breakdown.

**Request body:**

```json
{
  "department": "Sales",
  "period": "Q1 2024",
  "activities": [
    { "employeeName": "Alice Johnson", "activityType": "Cold Call", "date": "2024-01-08", "duration": "25 minutes", "description": "Outbound call to prospect", "result": "Meeting scheduled" }
  ]
}
```

**Response:**

```json
{
  "department": "Sales",
  "period": "Q1 2024",
  "totalActivities": 10,
  "uniqueEmployees": 4,
  "keyFindings": ["High call volume", "Improved conversion"],
  "categoryBreakdown": [{ "category": "Calls", "count": 6, "percentage": 60.0 }],
  "trends": [{ "indicator": "Activity Volume", "direction": "Up", "description": "10% increase" }],
  "summary": "Strong quarter for sales activities."
}
```

### Opportunity Analysis

```
POST /api/intelligence/opportunity-analysis
```

Analyzes a sales opportunity and predicts win probability with competitive positioning.

**Request body:**

```json
{
  "accountName": "Meridian Healthcare",
  "dealValue": 250000,
  "stage": "Proposal Sent",
  "expectedCloseDate": "2024-04-30",
  "competitorInfo": "Competing against HealthTech Pro (incumbent)",
  "notes": "Champion is the CTO. CFO cautious about switching costs.",
  "activities": [
    { "type": "Demo", "date": "2024-01-24", "description": "Full platform demo", "contactPerson": "Dr. Patel, CTO" }
  ]
}
```

**Response:**

```json
{
  "opportunityId": "...",
  "accountName": "Meridian Healthcare",
  "winProbability": 65,
  "verdict": "Likely Win",
  "strengths": ["Strong champion", "Good product fit"],
  "weaknesses": ["Switching cost concerns"],
  "competitivePosition": "Leading",
  "recommendedStrategy": [{ "action": "Schedule exec meeting", "priority": "High", "rationale": "Build relationship" }],
  "nextSteps": ["Send ROI analysis", "Schedule follow-up"],
  "summary": "Deal is progressing well with strong champion support."
}
```

### Recommended Actions

```
POST /api/intelligence/recommended-actions
```

Generates prioritized action items based on business context, challenges, and goals.

**Request body:**

```json
{
  "businessArea": "Sales Operations",
  "currentChallenges": "Sales cycle lengthened from 45 to 68 days. Win rate dropped from 32% to 24%.",
  "availableResources": "12 sales reps, CRM platform, $50K quarterly budget",
  "goals": "Reduce sales cycle to under 50 days, improve win rate to 30%",
  "recentMetrics": "Q4 2023: Revenue $2.1M (target $2.5M). Average deal size: $45K."
}
```

**Response:**

```json
{
  "businessArea": "Sales Operations",
  "actions": [
    { "title": "Automate reporting", "priority": "High", "impact": "High", "effort": "Medium", "description": "Implement automated sales reports", "expectedOutcome": "Save 10 hours per week" }
  ],
  "quickWins": ["Update CRM templates", "Automate email reminders"],
  "longTermInitiatives": ["Implement AI-powered lead scoring"],
  "summary": "Several actionable improvements identified."
}
```

### Management Dashboard

```
POST /api/intelligence/dashboard
```

Runs multiple analyses in parallel and returns a combined dashboard summary. At least one input must be provided.

**Request body:**

```json
{
  "customer": { ... },
  "opportunity": { ... },
  "activities": { ... },
  "actionsContext": { ... }
}
```

Each field is optional (nullable). Provide only the analyses you need. The response includes only the analyses that were requested.

**Response:**

```json
{
  "generatedAt": "2024-03-15T10:30:00Z",
  "customerRisk": { ... },
  "activitySummary": { ... },
  "opportunityAnalysis": { ... },
  "recommendedActions": { ... }
}
```

### Authentication

Protected endpoints require an `X-Api-Key` header when API keys are configured.

Public endpoints (no auth required): `/api/health`, `/api/samples`, `/swagger`

### Error Responses

All errors follow [RFC 7807 Problem Details](https://tools.ietf.org/html/rfc7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "An unexpected error occurred",
  "status": 500,
  "detail": "An internal error occurred. Please try again later.",
  "instance": "/api/business-workflow/analyze",
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
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
| xUnit | 2.5.3 | Test framework | Clean syntax, good tooling integration |
| FluentAssertions | 6.12.2 | Test assertions | Readable assertion syntax |
| Moq | 4.20.72 | Mocking framework | Interface-based mocking for unit tests |
| Docker | Multi-stage | Containerization | Consistent deployment environments |
| GitHub Actions | - | CI/CD | Automated build, test, and artifact publishing |

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

### Phase 1 — Foundation *(complete)*

- [x] Create project repository and solution structure
- [x] Define initial business workflow model
- [x] Document architecture
- [x] Create API with health check endpoint
- [x] Add sample business data generator
- [x] Add unit and integration tests

### Phase 2 — AI Integration *(complete)*

- [x] Add AI analysis service with OpenAI Responses API
- [x] Add prompt management and construction
- [x] Handle AI failures and timeouts
- [x] Define structured AI output schemas (JSON)
- [x] Add input validation and sanitization

### Phase 3 — Production Readiness *(complete)*

- [x] Authentication and API key management
- [x] Structured logging with correlation IDs
- [x] Health monitoring and metrics
- [x] Docker support
- [x] CI/CD pipeline (GitHub Actions)
- [x] Error responses following RFC 7807 (Problem Details)

### Phase 4 — Business Intelligence *(complete)*

- [x] Customer risk scoring
- [x] Activity summarization
- [x] Opportunity win/loss analysis
- [x] Recommended next actions engine
- [x] Management dashboard endpoints

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
