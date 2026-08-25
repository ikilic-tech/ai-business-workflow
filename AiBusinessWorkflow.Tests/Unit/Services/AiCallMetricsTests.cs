using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

public class AiCallMetricsTests
{
    [Fact]
    public void GetSummary_WithNoRecords_ShouldReturnZeros()
    {
        var metrics = new AiCallMetrics();
        var summary = metrics.GetSummary();

        summary.TotalCalls.Should().Be(0);
        summary.SuccessfulCalls.Should().Be(0);
        summary.FailedCalls.Should().Be(0);
    }

    [Fact]
    public void GetSummary_WithSuccessfulCalls_ShouldCalculateCorrectly()
    {
        var metrics = new AiCallMetrics();
        metrics.Record("customer-risk", 100, true);
        metrics.Record("customer-risk", 200, true);
        metrics.Record("opportunity-analysis", 150, true);

        var summary = metrics.GetSummary();

        summary.TotalCalls.Should().Be(3);
        summary.SuccessfulCalls.Should().Be(3);
        summary.FailedCalls.Should().Be(0);
        summary.AverageLatencyMs.Should().Be(150);
        summary.MinLatencyMs.Should().Be(100);
        summary.MaxLatencyMs.Should().Be(200);
    }

    [Fact]
    public void GetSummary_WithMixedResults_ShouldTrackFailures()
    {
        var metrics = new AiCallMetrics();
        metrics.Record("customer-risk", 100, true);
        metrics.Record("customer-risk", 50, false);

        var summary = metrics.GetSummary();

        summary.TotalCalls.Should().Be(2);
        summary.SuccessfulCalls.Should().Be(1);
        summary.FailedCalls.Should().Be(1);
    }

    [Fact]
    public void GetSummary_ShouldGroupByOperation()
    {
        var metrics = new AiCallMetrics();
        metrics.Record("customer-risk", 100, true);
        metrics.Record("customer-risk", 200, true);
        metrics.Record("opportunity-analysis", 300, true);

        var summary = metrics.GetSummary();

        summary.ByOperation.Should().ContainKey("customer-risk");
        summary.ByOperation.Should().ContainKey("opportunity-analysis");
        summary.ByOperation["customer-risk"].CallCount.Should().Be(2);
        summary.ByOperation["customer-risk"].AverageLatencyMs.Should().Be(150);
        summary.ByOperation["customer-risk"].SuccessRate.Should().Be(100);
    }

    [Fact]
    public void Reset_ShouldClearAllRecords()
    {
        var metrics = new AiCallMetrics();
        metrics.Record("customer-risk", 100, true);
        metrics.Reset();

        var summary = metrics.GetSummary();
        summary.TotalCalls.Should().Be(0);
    }

    [Fact]
    public void GetSummary_P95Latency_ShouldCalculateCorrectly()
    {
        var metrics = new AiCallMetrics();
        for (var i = 1; i <= 100; i++)
            metrics.Record("test", i * 10, true);

        var summary = metrics.GetSummary();

        summary.P95LatencyMs.Should().BeGreaterOrEqualTo(950);
    }
}
