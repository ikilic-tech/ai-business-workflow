using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

/// <summary>
/// Validates AI-generated response objects after deserialization.
/// AI output is treated as untrusted input — fields are checked for
/// required values, valid ranges, and acceptable enum values.
/// </summary>
public static class AiResponseValidator
{
    private static readonly HashSet<string> ValidRiskLevels = new(StringComparer.OrdinalIgnoreCase)
        { "Low", "Medium", "High", "Critical" };

    private static readonly HashSet<string> ValidChurnProbabilities = new(StringComparer.OrdinalIgnoreCase)
        { "Low", "Medium", "High" };

    private static readonly HashSet<string> ValidEngagementTrends = new(StringComparer.OrdinalIgnoreCase)
        { "Increasing", "Stable", "Declining" };

    private static readonly HashSet<string> ValidVerdicts = new(StringComparer.OrdinalIgnoreCase)
        { "Strong Win", "Likely Win", "Toss-Up", "At Risk", "Likely Loss" };

    private static readonly HashSet<string> ValidCompetitivePositions = new(StringComparer.OrdinalIgnoreCase)
        { "Leading", "Competitive", "Behind", "Unknown" };

    private static readonly HashSet<string> ValidPriorities = new(StringComparer.OrdinalIgnoreCase)
        { "Low", "Medium", "High", "Critical" };

    private static readonly HashSet<string> ValidRatings = new(StringComparer.OrdinalIgnoreCase)
        { "Low", "Medium", "High", "Very High" };

    private static readonly HashSet<string> ValidDirections = new(StringComparer.OrdinalIgnoreCase)
        { "Up", "Down", "Stable" };

    public static void Validate(CustomerRiskAssessment result)
    {
        result.RiskScore = ClampScore(result.RiskScore);

        if (!string.IsNullOrEmpty(result.RiskLevel) && !ValidRiskLevels.Contains(result.RiskLevel))
            result.RiskLevel = "Medium";

        if (!string.IsNullOrEmpty(result.ChurnProbability) && !ValidChurnProbabilities.Contains(result.ChurnProbability))
            result.ChurnProbability = "Medium";

        if (!string.IsNullOrEmpty(result.EngagementTrend) && !ValidEngagementTrends.Contains(result.EngagementTrend))
            result.EngagementTrend = "Stable";

        result.RiskFactors ??= new List<RiskFactor>();
        result.RecommendedActions ??= new List<string>();
        result.Summary ??= string.Empty;
    }

    public static void Validate(OpportunityAnalysisResult result)
    {
        result.WinProbability = ClampScore(result.WinProbability);

        if (!string.IsNullOrEmpty(result.Verdict) && !ValidVerdicts.Contains(result.Verdict))
            result.Verdict = "Toss-Up";

        if (!string.IsNullOrEmpty(result.CompetitivePosition) && !ValidCompetitivePositions.Contains(result.CompetitivePosition))
            result.CompetitivePosition = "Unknown";

        result.Strengths ??= new List<string>();
        result.Weaknesses ??= new List<string>();
        result.RecommendedStrategy ??= new List<StrategyItem>();
        result.NextSteps ??= new List<string>();
        result.Summary ??= string.Empty;
    }

    public static void Validate(ActivitySummaryReport result)
    {
        if (result.TotalActivities < 0)
            result.TotalActivities = 0;

        if (result.UniqueEmployees < 0)
            result.UniqueEmployees = 0;

        result.KeyFindings ??= new List<string>();
        result.CategoryBreakdown ??= new List<CategoryStat>();
        result.Trends ??= new List<TrendItem>();
        result.Summary ??= string.Empty;

        foreach (var trend in result.Trends)
        {
            if (!string.IsNullOrEmpty(trend.Direction) && !ValidDirections.Contains(trend.Direction))
                trend.Direction = "Stable";
        }
    }

    public static void Validate(RecommendedActionsReport result)
    {
        result.Actions ??= new List<ActionItem>();
        result.QuickWins ??= new List<string>();
        result.LongTermInitiatives ??= new List<string>();
        result.Summary ??= string.Empty;

        foreach (var action in result.Actions)
        {
            if (!string.IsNullOrEmpty(action.Priority) && !ValidPriorities.Contains(action.Priority))
                action.Priority = "Medium";
        }
    }

    public static void Validate(BusinessProcessAnalysis result)
    {
        if (result.Efficiency != null)
        {
            result.Efficiency.Score = ClampScore(result.Efficiency.Score);

            if (!string.IsNullOrEmpty(result.Efficiency.Rating) && !ValidRatings.Contains(result.Efficiency.Rating))
                result.Efficiency.Rating = "Medium";
        }

        if (!string.IsNullOrEmpty(result.OverallRiskLevel) && !ValidRiskLevels.Contains(result.OverallRiskLevel))
            result.OverallRiskLevel = "Medium";

        result.Bottlenecks ??= new List<Bottleneck>();
        result.Recommendations ??= new List<Recommendation>();
        result.AutomationOpportunities ??= new List<AutomationOpportunity>();
        result.Summary ??= string.Empty;
    }

    private static int ClampScore(int score) => Math.Clamp(score, 0, 100);
}
