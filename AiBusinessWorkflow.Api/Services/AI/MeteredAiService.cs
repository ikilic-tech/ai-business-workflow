using System.Diagnostics;
using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

public sealed class MeteredAiService : IAiService
{
    private readonly AiService _inner;
    private readonly AiCallMetrics _metrics;

    public MeteredAiService(AiService inner, AiCallMetrics metrics)
    {
        _inner = inner;
        _metrics = metrics;
    }

    public async Task<string> TestAiAsync(CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("test-ai", () => _inner.TestAiAsync(cancellationToken));
    }

    public async Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process, CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("business-workflow", () => _inner.AnalyzeBusinessProcessAsync(process, cancellationToken));
    }

    public async Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer, CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("customer-risk", () => _inner.AssessCustomerRiskAsync(customer, cancellationToken));
    }

    public async Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request, CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("activity-summary", () => _inner.SummarizeActivitiesAsync(request, cancellationToken));
    }

    public async Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("opportunity-analysis", () => _inner.AnalyzeOpportunityAsync(opportunity, cancellationToken));
    }

    public async Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request, CancellationToken cancellationToken = default)
    {
        return await MeasureAsync("recommended-actions", () => _inner.GenerateRecommendedActionsAsync(request, cancellationToken));
    }

    private async Task<T> MeasureAsync<T>(string operation, Func<Task<T>> action)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await action();
            sw.Stop();
            _metrics.Record(operation, sw.ElapsedMilliseconds, success: true);
            return result;
        }
        catch
        {
            sw.Stop();
            _metrics.Record(operation, sw.ElapsedMilliseconds, success: false);
            throw;
        }
    }
}
