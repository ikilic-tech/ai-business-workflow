using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class RecommendedActionsReport
{
    [JsonPropertyName("businessArea")]
    public string BusinessArea { get; set; } = string.Empty;

    [JsonPropertyName("actions")]
    public List<ActionItem> Actions { get; set; } = new();

    [JsonPropertyName("quickWins")]
    public List<string> QuickWins { get; set; } = new();

    [JsonPropertyName("longTermInitiatives")]
    public List<string> LongTermInitiatives { get; set; } = new();

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class ActionItem
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

    [JsonPropertyName("expectedOutcome")]
    public string ExpectedOutcome { get; set; } = string.Empty;
}