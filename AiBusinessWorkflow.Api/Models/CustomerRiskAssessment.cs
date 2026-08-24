using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class CustomerRiskAssessment
{
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [JsonPropertyName("riskScore")]
    public int RiskScore { get; set; }

    [JsonPropertyName("riskLevel")]
    public string RiskLevel { get; set; } = string.Empty;

    [JsonPropertyName("churnProbability")]
    public string ChurnProbability { get; set; } = string.Empty;

    [JsonPropertyName("engagementTrend")]
    public string EngagementTrend { get; set; } = string.Empty;

    [JsonPropertyName("riskFactors")]
    public List<RiskFactor> RiskFactors { get; set; } = new();

    [JsonPropertyName("recommendedActions")]
    public List<string> RecommendedActions { get; set; } = new();

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class RiskFactor
{
    [JsonPropertyName("factor")]
    public string Factor { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("impact")]
    public string Impact { get; set; } = string.Empty;
}