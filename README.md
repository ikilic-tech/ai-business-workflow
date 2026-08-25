# AI Business Workflow

**AI Native** — A .NET 8 Web API that uses AI to analyze business data and turn it into actionable insights: risk detection, bottleneck identification, optimization suggestions, automation opportunities, and recommended next steps.

> **AI-native engineering experiment:** AI is used not only as a runtime capability inside the product, but also as a first-class engineering capability across planning, architecture, implementation, testing, security, documentation and iteration. Humans remain responsible for direction, architecture, review and final decisions.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![OpenAI](https://img.shields.io/badge/AI-OpenAI-412991)](https://openai.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## Overview

Most business applications are good at collecting data: customer records, sales activities, visit notes, opportunities and follow-up dates. But they are not always good at telling people what to do with it.

This project closes that gap by taking structured business data through a REST API, running it through an AI analysis layer, and returning useful intelligence:

- efficiency analysis
- bottleneck detection
- customer risk scoring
- activity summarization
- opportunity win/loss analysis
- optimization recommendations
- automation opportunities
- prioritized next actions

```text
Business Data
      ↓
Validation & Preparation
      ↓
AI Analysis
      ↓
Structured Insights
      ↓
Recommended Actions
      ↓
Human Decision
```

The principle is simple:

> If AI output does not help someone make a decision or take an action, it is not doing its job.

## What makes this AI-native?

Traditional AI-assisted development often looks like:

```text
Human → Code → AI Assistant
```

This project explores a broader model:

```text
Problem Definition
       ↓
AI-assisted Exploration
       ↓
Human Architectural Decision
       ↓
AI-assisted Implementation
       ↓
Automated Tests
       ↓
AI-assisted Review
       ↓
Security Validation
       ↓
Human Approval
       ↓
CI/CD
       ↓
Measurement & Feedback
```

AI therefore participates in both:

1. **The product:** business intelligence and decision support.
2. **The engineering workflow:** architecture, implementation, testing, security and documentation.

See [AI_NATIVE.md](AI_NATIVE.md).

---

# Key Features

### Business intelligence

- Business process analysis via OpenAI Responses API
- Structured JSON output
- Efficiency ratings
- Bottleneck detection
- Optimization recommendations
- Automation opportunities
- Customer risk scoring based on activity patterns and payment history
- Activity summarization for management reporting
- Opportunity win/loss analysis with competitive positioning
- Recommended next actions engine with prioritized action items
- Management dashboard endpoint running independent analyses in parallel

### Engineering

- Configurable model selection
- AI provider abstraction through `IAiService`
- Input validation with DataAnnotations
- Synthetic/sample business data generator
- Health checks with AI connectivity and memory monitoring
- API key authentication
- Correlation ID tracking
- RFC 7807 ProblemDetails error handling
- Docker multi-stage build
- GitHub Actions CI/CD
- Swagger/OpenAPI
- OWASP-oriented security hardening
- 214 unit and integration tests at the current project baseline

### AI-native engineering

The project was intentionally developed with Claude Code as the primary AI engineering environment.

The documented workflow covers:

- natural-language planning
- architectural exploration
- code generation
- iterative build/test loops
- debugging
- refactoring
- test generation
- security review
- documentation

The human role is direction, review and decision-making.

See [AI_NATIVE.md](AI_NATIVE.md).

---

# Architecture

The application uses a layered architecture with the AI provider behind an interface.

```text
Middleware Pipeline
    ↓
API Layer
    ↓
Service Layer
    ↓
IAiService
    ↓
OpenAI Responses API
    ↓
Structured Output Models
    ↓
Business Intelligence Results
```

### Middleware

- Correlation ID
- Exception handling
- API key authentication

### API layer

Business endpoints include:

```text
POST /api/business-workflow/analyze

POST /api/intelligence/customer-risk
POST /api/intelligence/activity-summary
POST /api/intelligence/opportunity-analysis
POST /api/intelligence/recommended-actions
POST /api/intelligence/dashboard
```

Infrastructure/sample endpoints include:

```text
GET /api/health
GET /api/ai/test
GET /api/samples
GET /api/samples/{index}
GET /api/samples/customers
GET /api/samples/customers/{index}
GET /api/samples/opportunities
GET /api/samples/opportunities/{index}
GET /api/samples/activities
GET /api/samples/actions-context
```

### Service layer

`IAiService` provides operations for:

- business process analysis
- customer risk assessment
- activity summarization
- opportunity analysis
- recommended actions
- AI connectivity testing

The abstraction allows the business layer to remain independent of a specific AI provider.

See [ARCHITECTURE.md](ARCHITECTURE.md).

---

# Getting Started

## Prerequisites

- .NET 8 SDK
- OpenAI API key
- Docker (optional)

## Installation

```bash
git clone https://github.com/ikilic-tech/ai-business-workflow.git
cd ai-business-workflow
dotnet restore
```

## Configuration

Create a local configuration file and provide the AI key.

```json
{
  "AI": {
    "Provider": "OpenAI",
    "Model": "gpt-4o",
    "ApiKey": "sk-your-api-key-here"
  },
  "Authentication": {
    "ApiKeys": [
      "your-api-key-here"
    ]
  }
}
```

Do not commit API keys.

In Development, authentication can be skipped when API keys are not configured. In non-Development environments, API keys must be configured for authenticated requests.

## Run

```bash
cd AiBusinessWorkflow.Api
dotnet run
```

Default development endpoints:

```text
http://localhost:5221
http://localhost:5221/swagger
```

## Docker

```bash
AI_API_KEY=sk-your-key docker compose up --build
```

## Tests

```bash
dotnet test
```

---

# API Reference

## Health Check

```text
GET /api/health
```

Returns application health, AI connectivity and memory information.

## AI Connection Test

```text
GET /api/ai/test
```

Tests the configured AI connection.

## Sample Data

```text
GET /api/samples
GET /api/samples/{index}

GET /api/samples/customers
GET /api/samples/customers/{index}
GET /api/samples/opportunities
GET /api/samples/opportunities/{index}
GET /api/samples/activities
GET /api/samples/actions-context
```

The sample data includes business-process scenarios and synthetic BI data.

## Business Process Analysis

```text
POST /api/business-workflow/analyze
```

Analyzes a business process and returns structured optimization insights.

Typical output includes:

- process efficiency
- bottlenecks
- recommendations
- automation opportunities
- overall risk
- summary

## Customer Risk Assessment

```text
POST /api/intelligence/customer-risk
```

Assesses customer churn risk from profile, payment history and activity patterns.

Typical output:

```json
{
  "riskScore": 25,
  "riskLevel": "Low",
  "churnProbability": "Low",
  "engagementTrend": "Increasing",
  "riskFactors": [],
  "recommendedActions": [],
  "summary": "..."
}
```

## Activity Summary

```text
POST /api/intelligence/activity-summary
```

Summarizes activity volume, trends, categories and key findings for a department and period.

## Opportunity Analysis

```text
POST /api/intelligence/opportunity-analysis
```

Analyzes a sales opportunity and returns:

- win probability
- verdict
- strengths
- weaknesses
- competitive position
- recommended strategy
- next steps

## Recommended Actions

```text
POST /api/intelligence/recommended-actions
```

Generates prioritized actions from business context, challenges, resources and goals.

## Management Dashboard

```text
POST /api/intelligence/dashboard
```

Runs independent intelligence analyses in parallel and returns a combined dashboard result.

Inputs are optional; at least one analysis context must be supplied.

## Authentication

Protected endpoints use:

```http
X-Api-Key: your-api-key
```

Public endpoints include health, samples and Swagger according to the current configuration.

## Errors

Errors follow RFC 7807 Problem Details and include a correlation ID where applicable.

---

# Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 8.0 LTS | Runtime and Web API |
| C# | 12 | Primary language |
| OpenAI SDK | 2.13.0 | AI integration |
| ASP.NET Core | 8.0 | Web framework |
| Swagger / Swashbuckle | 6.6.2 | API documentation |
| xUnit | 2.5.3 | Testing |
| FluentAssertions | 6.12.2 | Assertions |
| Moq | 4.20.72 | Mocking |
| Docker | Multi-stage | Containerization |
| GitHub Actions | — | CI/CD |

## Technology decisions

### Responses API over Chat Completions

The Responses API provides a suitable interface for structured, machine-readable AI output.

### .NET 8 over .NET 9

The project prioritizes the LTS release and stability.

### Provider abstraction

`IAiService` separates business logic from the AI provider.

### No ORM/database in the current phase

The current phase focuses on the AI analysis pipeline. Persistence should be introduced when historical data, evaluation datasets or operational requirements justify it.

---

# Design Principles

- **AI should be useful, not impressive.**
- **Humans stay in the loop.**
- **Structured output is preferred over unstructured prose.**
- **Start small and measure before expanding.**
- **Security is part of the AI architecture.**
- **Synthetic data is used in the public repository.**
- **Claims should be backed by reproducible evidence.**
- **AI-generated work should remain reviewable and testable.**

---

# AI-Native Development

This project was built through deliberate human-AI collaboration.

The engineering workflow used Claude Code for:

- planning
- architecture exploration
- implementation
- debugging
- test generation
- refactoring
- security review
- documentation

The human role remained:

- defining goals
- making architectural decisions
- reviewing implementation
- validating behaviour
- approving changes

The project should not be interpreted as a claim that AI independently produced a production system without engineering oversight.

The purpose is to document and evaluate a practical **AI-native software engineering workflow**.

See [AI_NATIVE.md](AI_NATIVE.md).

---

# Evaluation

The project includes a reproducible evaluation framework covering:

- output validity (structured output validation via AiResponseValidator)
- business-intelligence quality (synthetic datasets for all 5 analysis types)
- regression behaviour (AI regression tests and prompt injection tests)

Planned but not yet implemented:

- latency and cost measurement
- benchmark harness for reproducible experiments
- AI-vs-deterministic-baseline comparisons
- adversarial prompt evaluation

No benchmark number will be published until it is produced by a reproducible experiment.

See [EVALUATION.md](EVALUATION.md).

---

# Security

The current security hardening includes:

- prompt injection protection
- collection size limits
- request body size limits
- timing-safe API-key comparison
- non-Development authentication enforcement
- security response headers
- dashboard timeout handling
- correlation ID validation

Future work focuses on making the threat model, evaluation and security verification externally auditable.

See [SECURITY.md](SECURITY.md).

---

# Roadmap

## Phase 1 — Foundation — Complete

- [x] Repository and solution structure
- [x] Initial business workflow model
- [x] Architecture documentation
- [x] Health check
- [x] Sample business data
- [x] Unit/integration tests

## Phase 2 — AI Integration — Complete

- [x] OpenAI Responses API
- [x] Prompt construction
- [x] AI failure/timeout handling
- [x] Structured JSON output
- [x] Input validation/sanitization

## Phase 3 — Production Readiness — Complete

- [x] API authentication
- [x] Correlation IDs
- [x] Health monitoring
- [x] Docker
- [x] GitHub Actions
- [x] RFC 7807 errors

## Phase 4 — Business Intelligence — Complete

- [x] Customer risk scoring
- [x] Activity summarization
- [x] Opportunity analysis
- [x] Recommended actions
- [x] Management dashboard

## Phase 5 — AI-Native Engineering — Implemented

- [x] AI-native methodology documentation (AI_NATIVE.md)
- [x] Architecture Decision Records (ADR-001 through ADR-004)
- [x] AI evaluation dataset (synthetic datasets for all 5 analysis types)
- [x] Prompt/version strategy (versioned prompt classes in Prompts/)
- [x] AI regression tests (validation + prompt injection tests)
- [x] Structured output validation (AiResponseValidator)
- [ ] Reproducible benchmark harness
- [ ] Cost/latency measurement
- [ ] AI-vs-deterministic-baseline comparison
- [ ] Adversarial prompt evaluation

## Phase 6 — Open Source Maturity — Partially Implemented

- [x] CONTRIBUTING.md
- [x] CODE_OF_CONDUCT.md
- [x] Issue templates (bug report, feature request, security)
- [x] Pull-request template
- [x] Security policy (SECURITY.md)
- [x] Changelog (CHANGELOG.md)
- [ ] Good-first-issue list
- [ ] Release/versioning policy
- [ ] Public demo
- [ ] Community examples

## Phase 7 — External Impact — Goal

- [ ] External users and contributors
- [ ] GitHub stars/forks growth
- [ ] Technical articles
- [ ] Independent references and adoption evidence

## Phase 8 — Research & Publication

- [ ] AI-native engineering case study
- [ ] Benchmark publication
- [ ] Cost/quality/latency analysis
- [ ] Lessons learned

---

# Motivation

This project is motivated by a long-standing problem in enterprise software: organizations collect large amounts of operational data but often struggle to convert it into timely decisions.

AI changes this in two ways.

First, AI can operate at runtime as an intelligence layer over business data.

Second, AI changes how software itself can be engineered.

This project deliberately explores both dimensions:

> **AI as the product capability and AI as the engineering capability.**

The longer-term goal is to demonstrate that AI can be integrated into enterprise software as a well-architected, testable, secure and maintainable service — while also exploring how AI-native development changes the engineering workflow.

---

# Open Source & Contribution

Contributions are welcome.

Before large changes, open an issue describing:

- the problem
- proposed solution
- expected impact
- testing approach

See [CONTRIBUTING.md](CONTRIBUTING.md).

---

# License

MIT License.

Copyright (c) 2026 Ibrahim Kilic.
