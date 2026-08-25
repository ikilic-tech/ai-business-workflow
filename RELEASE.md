# Release & Versioning Policy

## Versioning

This project follows [Semantic Versioning 2.0.0](https://semver.org/).

```
MAJOR.MINOR.PATCH
```

- **MAJOR**: Breaking API changes (endpoint removal, request/response schema changes)
- **MINOR**: New features, new endpoints, new analysis types
- **PATCH**: Bug fixes, security patches, documentation updates

## Current Version

The project is in **pre-release** (`0.x.y`). During pre-release:

- Minor version increments may include breaking changes
- The API surface is not yet frozen

## Release Process

1. All changes go through the `main` branch
2. CI must pass (build, tests, security scan) before merge
3. Releases are tagged with `v{MAJOR}.{MINOR}.{PATCH}`
4. Release notes reference the corresponding CHANGELOG.md section

## Changelog

All notable changes are documented in [CHANGELOG.md](CHANGELOG.md). The changelog follows [Keep a Changelog](https://keepachangelog.com/) conventions.

## Prompt Versioning

AI prompt templates are independently versioned in `Prompts/` classes. Prompt version changes do not necessarily trigger a project version bump unless they change the API contract.

| Prompt | Current Version |
|--------|----------------|
| BusinessWorkflowPrompt | 1.0.0 |
| CustomerRiskPrompt | 1.0.0 |
| ActivitySummaryPrompt | 1.0.0 |
| OpportunityAnalysisPrompt | 1.0.0 |
| RecommendedActionsPrompt | 1.0.0 |

## Branching

- `main`: production-ready code
- Feature branches: `feature/{description}`
- Bug fix branches: `fix/{description}`

## Security Releases

Security patches are released as soon as possible after discovery. See [SECURITY.md](SECURITY.md) for the vulnerability reporting process.
