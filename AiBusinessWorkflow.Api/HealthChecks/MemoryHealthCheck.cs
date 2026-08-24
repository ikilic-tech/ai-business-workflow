using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AiBusinessWorkflow.Api.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    private const long DegradedThresholdBytes = 500 * 1024 * 1024; // 500 MB
    private const long UnhealthyThresholdBytes = 1024 * 1024 * 1024; // 1 GB

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocatedBytes = GC.GetTotalMemory(forceFullCollection: false);
        var allocatedMb = allocatedBytes / (1024.0 * 1024.0);

        var data = new Dictionary<string, object>
        {
            ["allocatedMB"] = Math.Round(allocatedMb, 2),
            ["gen0Collections"] = GC.CollectionCount(0),
            ["gen1Collections"] = GC.CollectionCount(1),
            ["gen2Collections"] = GC.CollectionCount(2)
        };

        if (allocatedBytes >= UnhealthyThresholdBytes)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                $"Memory usage is critical: {allocatedMb:F2} MB", data: data));
        }

        if (allocatedBytes >= DegradedThresholdBytes)
        {
            return Task.FromResult(HealthCheckResult.Degraded(
                $"Memory usage is elevated: {allocatedMb:F2} MB", data: data));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            $"Memory usage is normal: {allocatedMb:F2} MB", data: data));
    }
}
