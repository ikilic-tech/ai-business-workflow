using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class OpportunityAnalysisResult
{
    [JsonPropertyName("opportunityId")]
    public string OpportunityId { get; set; } = string.Empty;

    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = string.Empty;

    [JsonPropertyName("winProbability")]
    public int WinProbability { get; set; }

    [JsonPropertyName("verdict")]
    public string Verdict { get; set; } = string.Empty;

    [JsonPropertyName("strengths")]
    public List<string> Strengths { get; set; } = new();

    [JsonPropertyName("weaknesses")]
    public List<string> Weaknesses { get; set; } = new();

    [JsonPropertyName("competitivePosition")]
    public string CompetitivePosition { get; set; } = string.Empty;

    [JsonPropertyName("recommendedStrategy")]
    public List<StrategyItem> RecommendedStrategy { get; set; } = new();

    [JsonPropertyName("nextSteps")]
    public List<string> NextSteps { get; set; } = new();

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class StrategyItem
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("rationale")]
    public string Rationale { get; set; } = string.Empty;
}