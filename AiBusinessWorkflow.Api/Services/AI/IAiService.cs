using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

public interface IAiService
{
    Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process);
    Task<string> TestAiAsync();
    Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer);
    Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request);
    Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity);
    Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request);
}
