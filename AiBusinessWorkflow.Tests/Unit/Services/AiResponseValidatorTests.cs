using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

public class AiResponseValidatorTests
{
    // CustomerRiskAssessment validation

    [Fact]
    public void Validate_CustomerRisk_ClampsScoreOver100()
    {
        var result = new CustomerRiskAssessment { RiskScore = 150, RiskLevel = "High" };
        AiResponseValidator.Validate(result);
        result.RiskScore.Should().Be(100);
    }

    [Fact]
    public void Validate_CustomerRisk_ClampsScoreBelow0()
    {
        var result = new CustomerRiskAssessment { RiskScore = -10, RiskLevel = "Low" };
        AiResponseValidator.Validate(result);
        result.RiskScore.Should().Be(0);
    }

    [Fact]
    public void Validate_CustomerRisk_NormalizesInvalidRiskLevel()
    {
        var result = new CustomerRiskAssessment { RiskScore = 50, RiskLevel = "Super Dangerous" };
        AiResponseValidator.Validate(result);
        result.RiskLevel.Should().Be("Medium");
    }

    [Fact]
    public void Validate_CustomerRisk_PreservesValidRiskLevel()
    {
        var result = new CustomerRiskAssessment { RiskScore = 20, RiskLevel = "Low" };
        AiResponseValidator.Validate(result);
        result.RiskLevel.Should().Be("Low");
    }

    [Fact]
    public void Validate_CustomerRisk_NormalizesInvalidChurnProbability()
    {
        var result = new CustomerRiskAssessment { RiskScore = 50, ChurnProbability = "Very High" };
        AiResponseValidator.Validate(result);
        result.ChurnProbability.Should().Be("Medium");
    }

    [Fact]
    public void Validate_CustomerRisk_NormalizesInvalidEngagementTrend()
    {
        var result = new CustomerRiskAssessment { RiskScore = 50, EngagementTrend = "Skyrocketing" };
        AiResponseValidator.Validate(result);
        result.EngagementTrend.Should().Be("Stable");
    }

    [Fact]
    public void Validate_CustomerRisk_InitializesNullCollections()
    {
        var result = new CustomerRiskAssessment
        {
            RiskScore = 50,
            RiskFactors = null!,
            RecommendedActions = null!,
            Summary = null!
        };
        AiResponseValidator.Validate(result);
        result.RiskFactors.Should().NotBeNull();
        result.RecommendedActions.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }

    // OpportunityAnalysisResult validation

    [Fact]
    public void Validate_Opportunity_ClampsWinProbability()
    {
        var result = new OpportunityAnalysisResult { WinProbability = 120, Verdict = "Strong Win" };
        AiResponseValidator.Validate(result);
        result.WinProbability.Should().Be(100);
    }

    [Fact]
    public void Validate_Opportunity_NormalizesInvalidVerdict()
    {
        var result = new OpportunityAnalysisResult { WinProbability = 50, Verdict = "Guaranteed Win" };
        AiResponseValidator.Validate(result);
        result.Verdict.Should().Be("Toss-Up");
    }

    [Fact]
    public void Validate_Opportunity_PreservesValidVerdict()
    {
        var result = new OpportunityAnalysisResult { WinProbability = 70, Verdict = "Likely Win" };
        AiResponseValidator.Validate(result);
        result.Verdict.Should().Be("Likely Win");
    }

    [Fact]
    public void Validate_Opportunity_NormalizesInvalidCompetitivePosition()
    {
        var result = new OpportunityAnalysisResult { WinProbability = 50, CompetitivePosition = "Dominant" };
        AiResponseValidator.Validate(result);
        result.CompetitivePosition.Should().Be("Unknown");
    }

    [Fact]
    public void Validate_Opportunity_InitializesNullCollections()
    {
        var result = new OpportunityAnalysisResult
        {
            WinProbability = 50,
            Strengths = null!,
            Weaknesses = null!,
            RecommendedStrategy = null!,
            NextSteps = null!,
            Summary = null!
        };
        AiResponseValidator.Validate(result);
        result.Strengths.Should().NotBeNull();
        result.Weaknesses.Should().NotBeNull();
        result.RecommendedStrategy.Should().NotBeNull();
        result.NextSteps.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }

    // ActivitySummaryReport validation

    [Fact]
    public void Validate_ActivitySummary_ClampsNegativeTotals()
    {
        var result = new ActivitySummaryReport { TotalActivities = -5, UniqueEmployees = -1 };
        AiResponseValidator.Validate(result);
        result.TotalActivities.Should().Be(0);
        result.UniqueEmployees.Should().Be(0);
    }

    [Fact]
    public void Validate_ActivitySummary_NormalizesInvalidTrendDirection()
    {
        var result = new ActivitySummaryReport
        {
            Trends = new List<TrendItem>
            {
                new() { Indicator = "Volume", Direction = "Skyrocketing", Description = "test" }
            }
        };
        AiResponseValidator.Validate(result);
        result.Trends[0].Direction.Should().Be("Stable");
    }

    [Fact]
    public void Validate_ActivitySummary_PreservesValidDirection()
    {
        var result = new ActivitySummaryReport
        {
            Trends = new List<TrendItem>
            {
                new() { Indicator = "Volume", Direction = "Up", Description = "test" }
            }
        };
        AiResponseValidator.Validate(result);
        result.Trends[0].Direction.Should().Be("Up");
    }

    // RecommendedActionsReport validation

    [Fact]
    public void Validate_RecommendedActions_NormalizesInvalidPriority()
    {
        var result = new RecommendedActionsReport
        {
            Actions = new List<ActionItem>
            {
                new() { Title = "Test", Priority = "Urgent", Impact = "High", Effort = "Low", Description = "d", ExpectedOutcome = "o" }
            }
        };
        AiResponseValidator.Validate(result);
        result.Actions[0].Priority.Should().Be("Medium");
    }

    [Fact]
    public void Validate_RecommendedActions_PreservesValidPriority()
    {
        var result = new RecommendedActionsReport
        {
            Actions = new List<ActionItem>
            {
                new() { Title = "Test", Priority = "Critical", Impact = "High", Effort = "Low", Description = "d", ExpectedOutcome = "o" }
            }
        };
        AiResponseValidator.Validate(result);
        result.Actions[0].Priority.Should().Be("Critical");
    }

    [Fact]
    public void Validate_RecommendedActions_InitializesNullCollections()
    {
        var result = new RecommendedActionsReport
        {
            Actions = null!,
            QuickWins = null!,
            LongTermInitiatives = null!,
            Summary = null!
        };
        AiResponseValidator.Validate(result);
        result.Actions.Should().NotBeNull();
        result.QuickWins.Should().NotBeNull();
        result.LongTermInitiatives.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }

    // BusinessProcessAnalysis validation

    [Fact]
    public void Validate_BusinessProcess_ClampsEfficiencyScore()
    {
        var result = new BusinessProcessAnalysis
        {
            Efficiency = new EfficiencyAnalysis { Score = 110, Rating = "High", Explanation = "test" }
        };
        AiResponseValidator.Validate(result);
        result.Efficiency.Score.Should().Be(100);
    }

    [Fact]
    public void Validate_BusinessProcess_NormalizesInvalidRating()
    {
        var result = new BusinessProcessAnalysis
        {
            Efficiency = new EfficiencyAnalysis { Score = 75, Rating = "Excellent", Explanation = "test" }
        };
        AiResponseValidator.Validate(result);
        result.Efficiency.Rating.Should().Be("Medium");
    }

    [Fact]
    public void Validate_BusinessProcess_NormalizesInvalidOverallRiskLevel()
    {
        var result = new BusinessProcessAnalysis { OverallRiskLevel = "Extreme" };
        AiResponseValidator.Validate(result);
        result.OverallRiskLevel.Should().Be("Medium");
    }

    [Fact]
    public void Validate_BusinessProcess_InitializesNullCollections()
    {
        var result = new BusinessProcessAnalysis
        {
            Bottlenecks = null!,
            Recommendations = null!,
            AutomationOpportunities = null!,
            Summary = null!
        };
        AiResponseValidator.Validate(result);
        result.Bottlenecks.Should().NotBeNull();
        result.Recommendations.Should().NotBeNull();
        result.AutomationOpportunities.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }
}
