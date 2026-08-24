using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;

namespace AiBusinessWorkflow.Tests.Integration.Helpers;

public class FakeAiService : IAiService
{
    public Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process)
    {
        var analysis = new BusinessProcessAnalysis
        {
            ProcessId = process.Id,
            ProcessName = process.Name,
            Efficiency = new EfficiencyAnalysis
            {
                Score = 75,
                Rating = "Medium",
                Explanation = "Test efficiency analysis"
            },
            Bottlenecks = new List<Bottleneck>
            {
                new()
                {
                    Area = "Test Area",
                    Severity = "Medium",
                    Description = "Test bottleneck",
                    SuggestedFix = "Test fix"
                }
            },
            Recommendations = new List<Recommendation>
            {
                new()
                {
                    Title = "Test Recommendation",
                    Priority = "High",
                    Impact = "High",
                    Effort = "Low",
                    Description = "Test recommendation description"
                }
            },
            AutomationOpportunities = new List<AutomationOpportunity>
            {
                new()
                {
                    Process = "Test Process",
                    CurrentState = "Manual",
                    ProposedAutomation = "Automated",
                    EstimatedTimeSaving = "50%"
                }
            },
            OverallRiskLevel = "Low",
            Summary = "Test analysis summary"
        };

        return Task.FromResult(analysis);
    }

    public Task<string> TestAiAsync()
    {
        return Task.FromResult("Hello from fake AI service!");
    }
}
