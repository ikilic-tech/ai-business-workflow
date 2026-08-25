# Roadmap

This roadmap separates what already exists from what is planned.

## Phase 1 — Foundation — Complete

- [x] Repository and solution structure
- [x] Business workflow model
- [x] Architecture
- [x] Health endpoint
- [x] Sample data
- [x] Unit/integration tests

## Phase 2 — AI Integration — Complete

- [x] OpenAI Responses API
- [x] Prompt construction
- [x] Failure/timeout handling
- [x] Structured JSON output
- [x] Input validation and sanitization

## Phase 3 — Production Readiness — Complete

- [x] API authentication
- [x] Correlation IDs
- [x] Health monitoring
- [x] Docker
- [x] CI/CD
- [x] RFC 7807 errors
- [x] Security hardening

## Phase 4 — Business Intelligence — Complete

- [x] Customer risk scoring
- [x] Activity summarization
- [x] Opportunity analysis
- [x] Recommended actions
- [x] Management dashboard

## Phase 5 — AI-Native Engineering — Implemented

- [x] Add `AI_NATIVE.md`
- [x] Document human/AI responsibility boundaries
- [x] Add architecture decision records (ADR-001 through ADR-004)
- [x] Record reproducible AI development workflow
- [x] Add prompt/version strategy (versioned prompt classes in Prompts/)
- [x] Add AI regression tests (6 prompt injection regression tests)
- [x] Build evaluation dataset (synthetic datasets for all 5 analysis types)
- [x] Add structured output validation (AiResponseValidator)
- [x] Build benchmark harness (evaluation dataset runner with timing + validation)
- [x] Add adversarial prompt evaluation (13 tests, 10 attack scenarios, tag injection fix)
- [ ] Measure latency/cost/reliability (requires live AI provider)
- [ ] Compare AI against deterministic baseline (requires live AI provider)

## Phase 6 — Open Source Maturity — Partially Implemented

- [x] CONTRIBUTING.md
- [x] CODE_OF_CONDUCT.md
- [x] Issue templates (bug report, feature request, security)
- [x] Pull-request template
- [x] Security policy (SECURITY.md)
- [ ] Good-first-issue list (requires GitHub label setup)
- [x] Release/versioning policy (RELEASE.md)
- [x] Changelog (CHANGELOG.md)
- [ ] Public demo
- [ ] Community examples

## Phase 7 — External Impact — Goal

These are targets, not claims:

- [ ] External users
- [ ] External contributors
- [ ] External pull requests
- [ ] External integrations
- [ ] GitHub stars/forks growth
- [ ] Technical articles
- [ ] Community discussion
- [ ] Independent references
- [ ] Measured adoption

The goal is not to manufacture metrics. The goal is to make the project useful enough that independent activity occurs naturally.

## Phase 8 — Research & Publication

- [ ] AI-native engineering case study
- [ ] Benchmark publication
- [ ] Cost/quality/latency analysis
- [ ] Lessons learned
- [ ] Technical conference/community submission where appropriate
