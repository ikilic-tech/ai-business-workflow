# Project Implementation Checklist

## Architecture

- [x] Existing architecture preserved
- [x] AI provider remains abstracted (`IAiService`)
- [x] Business logic remains testable (Moq-based unit tests)
- [x] Structured output validated (parse methods with type safety)
- [x] Middleware pipeline maintained (CorrelationId, ExceptionHandling, ApiKeyAuth)
- [x] Endpoint organisation (Controllers + IEndpointRouteBuilder extensions)

## AI

- [x] Prompts are maintainable (versioned prompt classes)
- [x] AI output is treated as untrusted (validation after deserialization)
- [x] Regression tests exist (parse methods, malformed output, edge cases)
- [x] Evaluation framework exists (evaluation/datasets/ with synthetic scenarios)
- [x] Prompt injection protection (InputSanitizer + data/instruction separation)

## Security

- [x] No secrets committed (.gitignore, appsettings.Local.json excluded)
- [x] Input limits exist (DataAnnotations, MaxLength, Kestrel body size)
- [x] Authentication works (API key middleware, timing-safe comparison)
- [x] Security checks run in CI (dotnet-format, security scanning)
- [x] Prompt injection cases tested (InputSanitizerTests)

## Testing

- [x] Build passes
- [x] Unit tests pass
- [x] Integration tests pass
- [x] AI regression tests pass

## Open Source

- [x] Contribution guide (CONTRIBUTING.md)
- [x] Code of conduct (CODE_OF_CONDUCT.md)
- [x] Issue templates (.github/ISSUE_TEMPLATE/)
- [x] PR template (.github/pull_request_template.md)
- [x] Changelog (CHANGELOG.md)

## Documentation

- [x] README accurate
- [x] Architecture documented (ARCHITECTURE.md + ADRs)
- [x] Security documented (SECURITY.md)
- [x] Evaluation documented (EVALUATION.md + evaluation/README.md)
- [x] Roadmap accurate (ROADMAP.md)
- [x] AI-native methodology documented (AI_NATIVE.md)
