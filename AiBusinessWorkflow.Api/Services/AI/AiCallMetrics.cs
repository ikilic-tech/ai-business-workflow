using System.Collections.Concurrent;
using System.Diagnostics;

namespace AiBusinessWorkflow.Api.Services.AI;

public sealed class AiCallMetrics
{
    private const int MaxRecords = 10_000;
    private readonly ConcurrentQueue<AiCallRecord> _records = new();

    public void Record(string operation, long elapsedMs, bool success)
    {
        _records.Enqueue(new AiCallRecord
        {
            Operation = operation,
            ElapsedMs = elapsedMs,
            Success = success,
            Timestamp = DateTimeOffset.UtcNow
        });

        while (_records.Count > MaxRecords)
            _records.TryDequeue(out _);
    }

    public AiMetricsSummary GetSummary()
    {
        var records = _records.ToArray();
        if (records.Length == 0)
            return new AiMetricsSummary();

        var successful = records.Where(r => r.Success).ToArray();
        var failed = records.Where(r => !r.Success).ToArray();

        return new AiMetricsSummary
        {
            TotalCalls = records.Length,
            SuccessfulCalls = successful.Length,
            FailedCalls = failed.Length,
            AverageLatencyMs = successful.Length > 0 ? (long)successful.Average(r => r.ElapsedMs) : 0,
            P95LatencyMs = successful.Length > 0 ? Percentile(successful.Select(r => r.ElapsedMs), 95) : 0,
            MaxLatencyMs = successful.Length > 0 ? successful.Max(r => r.ElapsedMs) : 0,
            MinLatencyMs = successful.Length > 0 ? successful.Min(r => r.ElapsedMs) : 0,
            ByOperation = records
                .GroupBy(r => r.Operation)
                .ToDictionary(
                    g => g.Key,
                    g => new OperationMetrics
                    {
                        CallCount = g.Count(),
                        AverageLatencyMs = (long)g.Where(r => r.Success).DefaultIfEmpty().Average(r => r?.ElapsedMs ?? 0),
                        SuccessRate = g.Count(r => r.Success) / (double)g.Count() * 100
                    })
        };
    }

    public void Reset() => _records.Clear();

    private static long Percentile(IEnumerable<long> values, int percentile)
    {
        var sorted = values.OrderBy(v => v).ToArray();
        if (sorted.Length == 0) return 0;
        var index = (int)Math.Ceiling(percentile / 100.0 * sorted.Length) - 1;
        return sorted[Math.Max(0, Math.Min(index, sorted.Length - 1))];
    }
}

public record AiCallRecord
{
    public required string Operation { get; init; }
    public required long ElapsedMs { get; init; }
    public required bool Success { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}

public record AiMetricsSummary
{
    public int TotalCalls { get; init; }
    public int SuccessfulCalls { get; init; }
    public int FailedCalls { get; init; }
    public long AverageLatencyMs { get; init; }
    public long P95LatencyMs { get; init; }
    public long MaxLatencyMs { get; init; }
    public long MinLatencyMs { get; init; }
    public Dictionary<string, OperationMetrics> ByOperation { get; init; } = new();
}

public record OperationMetrics
{
    public int CallCount { get; init; }
    public long AverageLatencyMs { get; init; }
    public double SuccessRate { get; init; }
}
