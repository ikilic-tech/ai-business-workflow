# Changelog

All notable changes to this project are documented in this file.

## Phase 5 — AI-Native Engineering — Implemented

### Adversarial Prompt Evaluation

- Fixed InputSanitizer to block `</user_data>` and `<user_data>` tag injection (data boundary escape)
- Added 13 adversarial prompt evaluation tests covering XML boundary escape, instruction override, role-playing, system prompt extraction, JSON payload injection, delimiter confusion, combined multi-vector attacks
- Created adversarial evaluation dataset with 10 attack scenarios (evaluation/datasets/adversarial.json)

### Benchmark Harness

- Created evaluation dataset validation tests (structure, required fields, unique IDs, score ranges)
- Created benchmark harness running all 5 analysis types through API endpoints with timing measurement
- Harness validates response structure, scores, and required fields
- Designed for extension to live AI provider testing

### Latency/Cost Measurement

- Added AiCallMetrics for tracking call count, latency (avg, p95, min, max), success rate per operation
- Added MeteredAiService decorator wrapping IAiService with automatic timing
- Added GET /api/ai/metrics and POST /api/ai/metrics/reset endpoints
- All AI calls are automatically measured through the decorator pattern

### Deterministic Baseline

- Added DeterministicBaselineService implementing IAiService with rule-based analysis
- Uses keyword matching and simple heuristics for all 5 analysis types
- Enables comparison between AI and deterministic outputs

### Release Policy

- Created RELEASE.md with semantic versioning, release process, prompt versioning table

### Open Source

- Created 5 good-first-issue GitHub issues (#1-#5)
- Created examples/api-examples.sh with curl examples for all endpoints

### Documentation

- Moved all project documentation to repository root (AI_NATIVE.md, ARCHITECTURE.md, SECURITY.md, EVALUATION.md, ROADMAP.md, CONTRIBUTING.md, CODE_OF_CONDUCT.md, AI_EVIDENCE.md, IMPACT.md)
- Updated .gitignore to allow documentation markdown files
- Created Architecture Decision Records (ADR-001 through ADR-004)
- Created PROJECT_IMPLEMENTATION_CHECKLIST.md

### Evaluation Framework

- Created evaluation/ directory with synthetic datasets for all 5 analysis types
- Customer risk: 3 scenarios (low/medium/high risk)
- Opportunity analysis: 2 scenarios (strong/at-risk)
- Activity summary: 1 scenario (mixed types)
- Business process analysis: 2 scenarios (manual/optimised)
- Recommended actions: 1 scenario (declining performance)
- Added validation check scenario definitions
- Created evaluation/README.md documenting the framework

### Prompt Versioning

- Extracted all 5 prompt templates from AiService.cs into versioned classes in Prompts/
- Each prompt class has version, purpose, expected input, expected output metadata
- BusinessWorkflowPrompt, CustomerRiskPrompt, ActivitySummaryPrompt, OpportunityAnalysisPrompt, RecommendedActionsPrompt (all v1.0.0)

### Structured Output Validation

- Added AiResponseValidator that validates AI responses after deserialization
- Score clamping to 0-100 range
- Enum value normalization for invalid values
- Null collection initialization
- Integrated validation into all parse methods

### AI Regression Tests

- Added 22 AiResponseValidatorTests covering all validation rules
- Added 6 PromptInjectionRegressionTests covering code fence injection, template injection, system override injection, and user data tag verification
- Total test count: 214

### CI/CD Improvements

- Added security job to CI pipeline (dependency vulnerability scanning, secret detection)
- Added GitHub issue templates (bug report, feature request, security)
- Added pull request template

## Phase 4 — Business Intelligence — Complete

### Features

- Customer risk assessment endpoint
- Activity summary endpoint
- Opportunity analysis endpoint
- Recommended actions endpoint
- Management dashboard with parallel execution
- Business intelligence sample data

### Security Hardening

- Prompt injection protection with InputSanitizer
- Collection size limits (MaxLength)
- Request body size cap (5 MB Kestrel limit)
- Timing-safe API key comparison
- API key enforcement in non-Development environments
- Security response headers
- Dashboard timeout with 504 response
- Correlation ID format validation
- Minimal API endpoints organized into extension classes

## Phase 3 — Production Readiness — Complete

- API key authentication middleware
- Correlation ID tracking
- Health monitoring (AI connectivity, memory)
- Docker multi-stage build
- GitHub Actions CI/CD
- RFC 7807 ProblemDetails error handling

## Phase 2 — AI Integration — Complete

- OpenAI Responses API integration
- Prompt construction with JSON schema
- AI failure and timeout handling
- Structured JSON output with typed models
- Input validation and sanitization

## Phase 1 — Foundation — Complete

- .NET 8 Web API project structure
- Business workflow model
- Health check endpoint
- Sample business data generator
- Unit and integration tests
