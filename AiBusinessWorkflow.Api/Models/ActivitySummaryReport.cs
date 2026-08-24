using System.Text.Json.Serialization;

namespace AiBusinessWorkflow.Api.Models;

public class ActivitySummaryReport
{
    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;

    [JsonPropertyName("totalActivities")]
    public int TotalActivities { get; set; }

    [JsonPropertyName("uniqueEmployees")]
    public int UniqueEmployees { get; set; }

    [JsonPropertyName("keyFindings")]
    public List<string> KeyFindings { get; set; } = new();

    [JsonPropertyName("categoryBreakdown")]
    public List<CategoryStat> CategoryBreakdown { get; set; } = new();

    [JsonPropertyName("trends")]
    public List<TrendItem> Trends { get; set; } = new();

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;
}

public class CategoryStat
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("percentage")]
    public double Percentage { get; set; }
}

public class TrendItem
{
    [JsonPropertyName("indicator")]
    public string Indicator { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}