using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

public class AiServiceTests
{
    private const string ValidJson = """
        {
          "processId": "test-001",
          "processName": "Test Process",
          "efficiency": {
            "score": 72,
            "rating": "Medium",
            "explanation": "Process has moderate efficiency with room for improvement."
          },
          "bottlenecks": [
            {
              "area": "Manual Data Entry",
              "severity": "High",
              "description": "Data is entered manually causing delays.",
              "suggestedFix": "Implement OCR-based data extraction."
            }
          ],
          "recommendations": [
            {
              "title": "Automate Data Entry",
              "priority": "High",
              "impact": "High",
              "effort": "Medium",
              "description": "Replace manual data entry with automated extraction."
            }
          ],
          "automationOpportunities": [
            {
              "process": "Invoice Scanning",
              "currentState": "Manual review of each invoice",
              "proposedAutomation": "AI-powered invoice parsing",
              "estimatedTimeSaving": "60% reduction in processing time"
            }
          ],
          "overallRiskLevel": "Medium",
          "summary": "The process is functional but has significant optimization opportunities."
        }
        """;

    [Fact]
    public void ParseAnalysisResponse_WithValidJson_ShouldReturnCorrectAnalysis()
    {
        var result = AiService.ParseAnalysisResponse(ValidJson, "test-001", "Test Process");

        result.Should().NotBeNull();
        result.ProcessId.Should().Be("test-001");
        result.ProcessName.Should().Be("Test Process");
        result.Efficiency.Score.Should().Be(72);
        result.Efficiency.Rating.Should().Be("Medium");
        result.Bottlenecks.Should().HaveCount(1);
        result.Bottlenecks[0].Area.Should().Be("Manual Data Entry");
        result.Recommendations.Should().HaveCount(1);
        result.Recommendations[0].Title.Should().Be("Automate Data Entry");
        result.AutomationOpportunities.Should().HaveCount(1);
        result.OverallRiskLevel.Should().Be("Medium");
        result.Summary.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ParseAnalysisResponse_WithCodeFences_ShouldStripAndParse()
    {
        var wrapped = $"```json\n{ValidJson}\n```";
        var result = AiService.ParseAnalysisResponse(wrapped, "test-001", "Test Process");

        result.Should().NotBeNull();
        result.ProcessId.Should().Be("test-001");
        result.Efficiency.Score.Should().Be(72);
    }

    [Fact]
    public void ParseAnalysisResponse_WithMalformedJson_ShouldThrow()
    {
        var malformed = "{ this is not valid json }";
        var act = () => AiService.ParseAnalysisResponse(malformed, "test-001", "Test");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseAnalysisResponse_WithEmptyJson_ShouldThrow()
    {
        var act = () => AiService.ParseAnalysisResponse("", "test-001", "Test");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ParseAnalysisResponse_WithMissingFields_ShouldReturnDefaults()
    {
        var minimal = """
            {
              "efficiency": { "score": 50 },
              "bottlenecks": [],
              "recommendations": [],
              "automationOpportunities": []
            }
            """;

        var result = AiService.ParseAnalysisResponse(minimal, "id-1", "Name");

        result.ProcessId.Should().Be("id-1");
        result.ProcessName.Should().Be("Name");
        result.Efficiency.Score.Should().Be(50);
        result.Efficiency.Rating.Should().BeEmpty();
        result.Bottlenecks.Should().BeEmpty();
        result.OverallRiskLevel.Should().BeEmpty();
        result.Summary.Should().BeEmpty();
    }

    [Fact]
    public void ParseAnalysisResponse_ShouldOverrideProcessIdAndName()
    {
        var json = """
            {
              "processId": "wrong-id",
              "processName": "Wrong Name",
              "efficiency": { "score": 80 },
              "bottlenecks": [],
              "recommendations": [],
              "automationOpportunities": []
            }
            """;

        var result = AiService.ParseAnalysisResponse(json, "correct-id", "Correct Name");

        result.ProcessId.Should().Be("correct-id");
        result.ProcessName.Should().Be("Correct Name");
    }

    [Fact]
    public void ParseAnalysisResponse_WithMultipleBottlenecks_ShouldParseAll()
    {
        var json = """
            {
              "efficiency": { "score": 40, "rating": "Low", "explanation": "Poor" },
              "bottlenecks": [
                { "area": "Area1", "severity": "High", "description": "D1", "suggestedFix": "F1" },
                { "area": "Area2", "severity": "Medium", "description": "D2", "suggestedFix": "F2" },
                { "area": "Area3", "severity": "Low", "description": "D3", "suggestedFix": "F3" }
              ],
              "recommendations": [],
              "automationOpportunities": [],
              "overallRiskLevel": "High",
              "summary": "Needs work"
            }
            """;

        var result = AiService.ParseAnalysisResponse(json, "id", "name");

        result.Bottlenecks.Should().HaveCount(3);
        result.Bottlenecks[0].Area.Should().Be("Area1");
        result.Bottlenecks[2].Area.Should().Be("Area3");
    }

    [Fact]
    public void ParseAnalysisResponse_CodeFenceWithoutLanguage_ShouldStripAndParse()
    {
        var wrapped = $"```\n{ValidJson}\n```";
        var result = AiService.ParseAnalysisResponse(wrapped, "test-001", "Test");

        result.Should().NotBeNull();
        result.Efficiency.Score.Should().Be(72);
    }
}
