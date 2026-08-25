using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

public interface IAiService
{
    Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process, CancellationToken cancellationToken = default);
    Task<string> TestAiAsync(CancellationToken cancellationToken = default);
    Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer, CancellationToken cancellationToken = default);
    Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request, CancellationToken cancellationToken = default);
    Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity, CancellationToken cancellationToken = default);
    Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request, CancellationToken cancellationToken = default);
}
