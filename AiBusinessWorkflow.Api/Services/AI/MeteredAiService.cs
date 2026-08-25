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

    public async Task<string> TestAiAsync()
    {
        return await MeasureAsync("test-ai", () => _inner.TestAiAsync());
    }

    public async Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process)
    {
        return await MeasureAsync("business-workflow", () => _inner.AnalyzeBusinessProcessAsync(process));
    }

    public async Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer)
    {
        return await MeasureAsync("customer-risk", () => _inner.AssessCustomerRiskAsync(customer));
    }

    public async Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request)
    {
        return await MeasureAsync("activity-summary", () => _inner.SummarizeActivitiesAsync(request));
    }

    public async Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity)
    {
        return await MeasureAsync("opportunity-analysis", () => _inner.AnalyzeOpportunityAsync(opportunity));
    }

    public async Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request)
    {
        return await MeasureAsync("recommended-actions", () => _inner.GenerateRecommendedActionsAsync(request));
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
