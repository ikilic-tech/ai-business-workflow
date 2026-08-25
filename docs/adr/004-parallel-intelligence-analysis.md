# ADR-004: Parallel Intelligence Analysis

## Status

Accepted

## Date

2026-02-15

## Context

The management dashboard endpoint (`POST /api/intelligence/dashboard`) can execute up to four independent AI analyses:

1. Customer risk assessment
2. Activity summary
3. Opportunity analysis
4. Recommended actions

Each analysis involves a round-trip to the AI provider. Sequential execution would multiply total latency by the number of requested analyses.

## Problem

How should the dashboard execute multiple independent analyses efficiently while handling timeouts and partial failures?

## Decision

Use `Task.WhenAll()` to execute all requested analyses concurrently, with a `CancellationTokenSource` timeout:

```csharp
var tasks = new List<Task>();
// ... add tasks based on non-null request fields

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
await Task.WhenAll(tasks).WaitAsync(cts.Token);
```

Behaviour:
- Only non-null request fields trigger their corresponding analysis
- An empty request returns 400 Bad Request
- If all tasks complete within 60 seconds, the combined result is returned
- If the timeout is exceeded, a 504 Gateway Timeout is returned with a ProblemDetails body
- Each analysis runs independently; there are no data dependencies between them

## Alternatives Considered

### Sequential execution

Simpler but would multiply latency. For four analyses at ~3 seconds each, sequential execution would take ~12 seconds versus ~3 seconds parallel.

### Background jobs with polling

More complex. Appropriate for long-running batch operations but unnecessary for a synchronous dashboard request.

### Parallel with individual timeout per task

More granular control but adds complexity. A single overall timeout is simpler and sufficient for the current use case.

### Fire-and-forget with partial results

Return whatever completes within the timeout. Adds complexity to the response contract (partial success). Deferred to a future iteration if needed.

## Consequences

### Positive

- Dashboard latency is approximately the slowest single analysis, not the sum
- Timeout prevents indefinite waits
- Clean 504 response when timeout occurs
- Null fields in the response clearly indicate which analyses were not requested

### Negative

- A single slow analysis can delay the entire response
- Timeout of 60 seconds is a fixed value (not configurable per request)
- If one analysis fails, the entire dashboard request fails

### Risks

- Under high load, parallel requests to the AI provider may cause rate limiting
- The 60-second timeout may be too short for complex analyses or too long for user experience
