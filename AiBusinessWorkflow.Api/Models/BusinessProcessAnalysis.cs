using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class BusinessProcessAnalysis
{
    [JsonPropertyName("processId")]
    public string ProcessId { get; set; } = string.Empty;

    [JsonPropertyName("processName")]
    public string ProcessName { get; set; } = string.Empty;

    [JsonPropertyName("efficiency")]
    public EfficiencyAnalysis Efficiency { get; set; } = new();

    [JsonPropertyName("bottlenecks")]
    public List<Bottleneck> Bottlenecks { get; set; } = new();

    [JsonPropertyName("recommendations")]
    public List<Recommendation> Recommendations { get; set; } = new();

    [JsonPropertyName("automationOpportunities")]
    public List<AutomationOpportunity> AutomationOpportunities { get; set; } = new();

    [JsonPropertyName("overallRiskLevel")]
    public string OverallRiskLevel { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class EfficiencyAnalysis
{
    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = string.Empty;

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;
}

public class Bottleneck
{
    [JsonPropertyName("area")]
    public string Area { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("suggestedFix")]
    public string SuggestedFix { get; set; } = string.Empty;
}

public class Recommendation
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("impact")]
    public string Impact { get; set; } = string.Empty;

    [JsonPropertyName("effort")]
    public string Effort { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class AutomationOpportunity
{
    [JsonPropertyName("process")]
    public string Process { get; set; } = string.Empty;

    [JsonPropertyName("currentState")]
    public string CurrentState { get; set; } = string.Empty;

    [JsonPropertyName("proposedAutomation")]
    public string ProposedAutomation { get; set; } = string.Empty;

    [JsonPropertyName("estimatedTimeSaving")]
    public string EstimatedTimeSaving { get; set; } = string.Empty;
}
