using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

public class AiServiceBusinessIntelligenceTests
{
    #region ParseCustomerRiskResponse

    private const string ValidCustomerRiskJson = """
        {
          "customerId": "cust-001",
          "companyName": "Test Corp",
          "riskScore": 45,
          "riskLevel": "Medium",
          "churnProbability": "Low",
          "engagementTrend": "Stable",
          "riskFactors": [
            {
              "factor": "Late Payments",
              "severity": "Medium",
              "description": "Two late payments in past quarter",
              "impact": "Moderate financial risk"
            }
          ],
          "recommendedActions": ["Schedule account review", "Offer payment plan"],
          "summary": "Customer shows moderate risk with some warning signs."
        }
        """;

    [Fact]
    public void ParseCustomerRiskResponse_WithValidJson_ShouldReturnCorrectResult()
    {
        var result = AiService.ParseCustomerRiskResponse(ValidCustomerRiskJson, "cust-001", "Test Corp");

        result.Should().NotBeNull();
        result.CustomerId.Should().Be("cust-001");
        result.CompanyName.Should().Be("Test Corp");
        result.RiskScore.Should().Be(45);
        result.RiskLevel.Should().Be("Medium");
        result.ChurnProbability.Should().Be("Low");
        result.EngagementTrend.Should().Be("Stable");
        result.RiskFactors.Should().HaveCount(1);
        result.RiskFactors[0].Factor.Should().Be("Late Payments");
        result.RecommendedActions.Should().HaveCount(2);
        result.Summary.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ParseCustomerRiskResponse_WithCodeFences_ShouldStripAndParse()
    {
        var wrapped = $"```json\n{ValidCustomerRiskJson}\n```";
        var result = AiService.ParseCustomerRiskResponse(wrapped, "cust-001", "Test Corp");

        result.Should().NotBeNull();
        result.RiskScore.Should().Be(45);
    }

    [Fact]
    public void ParseCustomerRiskResponse_WithMalformedJson_ShouldThrow()
    {
        var act = () => AiService.ParseCustomerRiskResponse("{ not valid }", "id", "name");
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseCustomerRiskResponse_WithMissingFields_ShouldReturnDefaults()
    {
        var minimal = """{ "riskScore": 20, "riskFactors": [], "recommendedActions": [] }""";
        var result = AiService.ParseCustomerRiskResponse(minimal, "id-1", "Company");

        result.CustomerId.Should().Be("id-1");
        result.CompanyName.Should().Be("Company");
        result.RiskScore.Should().Be(20);
        result.RiskLevel.Should().BeEmpty();
        result.Summary.Should().BeEmpty();
    }

    [Fact]
    public void ParseCustomerRiskResponse_ShouldOverrideIdentifiers()
    {
        var json = """
            {
              "customerId": "wrong-id",
              "companyName": "Wrong Name",
              "riskScore": 50,
              "riskFactors": [],
              "recommendedActions": []
            }
            """;
        var result = AiService.ParseCustomerRiskResponse(json, "correct-id", "Correct Name");

        result.CustomerId.Should().Be("correct-id");
        result.CompanyName.Should().Be("Correct Name");
    }

    #endregion

    #region ParseActivitySummaryResponse

    private const string ValidActivitySummaryJson = """
        {
          "department": "Sales",
          "period": "Q1 2024",
          "totalActivities": 25,
          "uniqueEmployees": 5,
          "keyFindings": ["High call volume", "Improved conversion"],
          "categoryBreakdown": [
            { "category": "Calls", "count": 15, "percentage": 60.0 },
            { "category": "Meetings", "count": 10, "percentage": 40.0 }
          ],
          "trends": [
            { "indicator": "Activity Volume", "direction": "Up", "description": "10% increase" }
          ],
          "summary": "Strong quarter for sales activities."
        }
        """;

    [Fact]
    public void ParseActivitySummaryResponse_WithValidJson_ShouldReturnCorrectResult()
    {
        var result = AiService.ParseActivitySummaryResponse(ValidActivitySummaryJson, "Sales", "Q1 2024");

        result.Should().NotBeNull();
        result.Department.Should().Be("Sales");
        result.Period.Should().Be("Q1 2024");
        result.TotalActivities.Should().Be(25);
        result.UniqueEmployees.Should().Be(5);
        result.KeyFindings.Should().HaveCount(2);
        result.CategoryBreakdown.Should().HaveCount(2);
        result.CategoryBreakdown[0].Category.Should().Be("Calls");
        result.Trends.Should().HaveCount(1);
        result.Summary.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ParseActivitySummaryResponse_WithCodeFences_ShouldStripAndParse()
    {
        var wrapped = $"```json\n{ValidActivitySummaryJson}\n```";
        var result = AiService.ParseActivitySummaryResponse(wrapped, "Sales", "Q1 2024");

        result.Should().NotBeNull();
        result.TotalActivities.Should().Be(25);
    }

    [Fact]
    public void ParseActivitySummaryResponse_WithMalformedJson_ShouldThrow()
    {
        var act = () => AiService.ParseActivitySummaryResponse("{ bad json }", "Sales", "Q1");
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseActivitySummaryResponse_WithMissingFields_ShouldReturnDefaults()
    {
        var minimal = """{ "totalActivities": 10, "keyFindings": [], "categoryBreakdown": [], "trends": [] }""";
        var result = AiService.ParseActivitySummaryResponse(minimal, "Marketing", "Q2 2024");

        result.Department.Should().Be("Marketing");
        result.Period.Should().Be("Q2 2024");
        result.TotalActivities.Should().Be(10);
        result.Summary.Should().BeEmpty();
    }

    #endregion

    #region ParseOpportunityAnalysisResponse

    private const string ValidOpportunityJson = """
        {
          "opportunityId": "opp-001",
          "accountName": "Test Account",
          "winProbability": 65,
          "verdict": "Likely Win",
          "strengths": ["Strong champion", "Good fit"],
          "weaknesses": ["Price concerns"],
          "competitivePosition": "Leading",
          "recommendedStrategy": [
            { "action": "Schedule exec meeting", "priority": "High", "rationale": "Build relationship" }
          ],
          "nextSteps": ["Send proposal", "Follow up"],
          "summary": "Deal is progressing well."
        }
        """;

    [Fact]
    public void ParseOpportunityAnalysisResponse_WithValidJson_ShouldReturnCorrectResult()
    {
        var result = AiService.ParseOpportunityAnalysisResponse(ValidOpportunityJson, "opp-001", "Test Account");

        result.Should().NotBeNull();
        result.OpportunityId.Should().Be("opp-001");
        result.AccountName.Should().Be("Test Account");
        result.WinProbability.Should().Be(65);
        result.Verdict.Should().Be("Likely Win");
        result.Strengths.Should().HaveCount(2);
        result.Weaknesses.Should().HaveCount(1);
        result.CompetitivePosition.Should().Be("Leading");
        result.RecommendedStrategy.Should().HaveCount(1);
        result.RecommendedStrategy[0].Action.Should().Be("Schedule exec meeting");
        result.NextSteps.Should().HaveCount(2);
        result.Summary.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ParseOpportunityAnalysisResponse_WithCodeFences_ShouldStripAndParse()
    {
        var wrapped = $"```\n{ValidOpportunityJson}\n```";
        var result = AiService.ParseOpportunityAnalysisResponse(wrapped, "opp-001", "Test Account");

        result.Should().NotBeNull();
        result.WinProbability.Should().Be(65);
    }

    [Fact]
    public void ParseOpportunityAnalysisResponse_WithMalformedJson_ShouldThrow()
    {
        var act = () => AiService.ParseOpportunityAnalysisResponse("invalid", "id", "name");
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseOpportunityAnalysisResponse_ShouldOverrideIdentifiers()
    {
        var json = """
            {
              "opportunityId": "wrong",
              "accountName": "Wrong",
              "winProbability": 80,
              "strengths": [],
              "weaknesses": [],
              "recommendedStrategy": [],
              "nextSteps": []
            }
            """;
        var result = AiService.ParseOpportunityAnalysisResponse(json, "correct-id", "Correct Name");

        result.OpportunityId.Should().Be("correct-id");
        result.AccountName.Should().Be("Correct Name");
    }

    #endregion

    #region ParseRecommendedActionsResponse

    private const string ValidActionsJson = """
        {
          "businessArea": "Sales",
          "actions": [
            {
              "title": "Automate reporting",
              "priority": "High",
              "impact": "High",
              "effort": "Medium",
              "description": "Implement automated sales reports",
              "expectedOutcome": "Save 10 hours per week"
            }
          ],
          "quickWins": ["Update CRM templates"],
          "longTermInitiatives": ["Implement AI-powered lead scoring"],
          "summary": "Several actionable improvements identified."
        }
        """;

    [Fact]
    public void ParseRecommendedActionsResponse_WithValidJson_ShouldReturnCorrectResult()
    {
        var result = AiService.ParseRecommendedActionsResponse(ValidActionsJson, "Sales");

        result.Should().NotBeNull();
        result.BusinessArea.Should().Be("Sales");
        result.Actions.Should().HaveCount(1);
        result.Actions[0].Title.Should().Be("Automate reporting");
        result.Actions[0].Priority.Should().Be("High");
        result.Actions[0].ExpectedOutcome.Should().Be("Save 10 hours per week");
        result.QuickWins.Should().HaveCount(1);
        result.LongTermInitiatives.Should().HaveCount(1);
        result.Summary.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ParseRecommendedActionsResponse_WithCodeFences_ShouldStripAndParse()
    {
        var wrapped = $"```json\n{ValidActionsJson}\n```";
        var result = AiService.ParseRecommendedActionsResponse(wrapped, "Sales");

        result.Should().NotBeNull();
        result.Actions.Should().HaveCount(1);
    }

    [Fact]
    public void ParseRecommendedActionsResponse_WithMalformedJson_ShouldThrow()
    {
        var act = () => AiService.ParseRecommendedActionsResponse("not json", "Sales");
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseRecommendedActionsResponse_ShouldOverrideBusinessArea()
    {
        var json = """
            {
              "businessArea": "Wrong Area",
              "actions": [],
              "quickWins": [],
              "longTermInitiatives": []
            }
            """;
        var result = AiService.ParseRecommendedActionsResponse(json, "Correct Area");

        result.BusinessArea.Should().Be("Correct Area");
    }

    #endregion
}
