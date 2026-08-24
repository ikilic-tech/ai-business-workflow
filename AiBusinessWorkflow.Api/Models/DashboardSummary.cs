using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class DashboardSummary
{
    [JsonPropertyName("generatedAt")]
    public DateTime GeneratedAt { get; set; }

    [JsonPropertyName("customerRisk")]
    public CustomerRiskAssessment? CustomerRisk { get; set; }

    [JsonPropertyName("activitySummary")]
    public ActivitySummaryReport? ActivitySummary { get; set; }

    [JsonPropertyName("opportunityAnalysis")]
    public OpportunityAnalysisResult? OpportunityAnalysis { get; set; }

    [JsonPropertyName("recommendedActions")]
    public RecommendedActionsReport? RecommendedActions { get; set; }
}