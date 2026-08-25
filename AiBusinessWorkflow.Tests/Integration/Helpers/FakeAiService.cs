using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;

namespace AiBusinessWorkflow.Tests.Integration.Helpers;

public class FakeAiService : IAiService
{
    public Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process, CancellationToken cancellationToken = default)
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

    public Task<string> TestAiAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Hello from fake AI service!");
    }

    public Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer, CancellationToken cancellationToken = default)
    {
        var result = new CustomerRiskAssessment
        {
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            RiskScore = 35,
            RiskLevel = "Low",
            ChurnProbability = "Low",
            EngagementTrend = "Stable",
            RiskFactors = new List<RiskFactor>
            {
                new()
                {
                    Factor = "Payment Consistency",
                    Severity = "Low",
                    Description = "Payments are generally on time",
                    Impact = "Minimal risk"
                }
            },
            RecommendedActions = new List<string> { "Continue regular check-ins" },
            Summary = "Test customer risk assessment"
        };
        return Task.FromResult(result);
    }

    public Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request, CancellationToken cancellationToken = default)
    {
        var result = new ActivitySummaryReport
        {
            Department = request.Department,
            Period = request.Period,
            TotalActivities = request.Activities.Count,
            UniqueEmployees = request.Activities.Select(a => a.EmployeeName).Distinct().Count(),
            KeyFindings = new List<string> { "Test finding" },
            CategoryBreakdown = new List<CategoryStat>
            {
                new() { Category = "Sales Call", Count = 5, Percentage = 50.0 }
            },
            Trends = new List<TrendItem>
            {
                new() { Indicator = "Activity Volume", Direction = "Up", Description = "Increasing activity" }
            },
            Summary = "Test activity summary"
        };
        return Task.FromResult(result);
    }

    public Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        var result = new OpportunityAnalysisResult
        {
            OpportunityId = opportunity.OpportunityId,
            AccountName = opportunity.AccountName,
            WinProbability = 70,
            Verdict = "Likely Win",
            Strengths = new List<string> { "Strong relationship" },
            Weaknesses = new List<string> { "Price sensitivity" },
            CompetitivePosition = "Leading",
            RecommendedStrategy = new List<StrategyItem>
            {
                new() { Action = "Schedule demo", Priority = "High", Rationale = "Build momentum" }
            },
            NextSteps = new List<string> { "Follow up next week" },
            Summary = "Test opportunity analysis"
        };
        return Task.FromResult(result);
    }

    public Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request, CancellationToken cancellationToken = default)
    {
        var result = new RecommendedActionsReport
        {
            BusinessArea = request.BusinessArea,
            Actions = new List<ActionItem>
            {
                new()
                {
                    Title = "Improve follow-up process",
                    Priority = "High",
                    Impact = "High",
                    Effort = "Medium",
                    Description = "Standardize follow-up procedures",
                    ExpectedOutcome = "Better conversion rates"
                }
            },
            QuickWins = new List<string> { "Automate email reminders" },
            LongTermInitiatives = new List<string> { "CRM integration" },
            Summary = "Test recommended actions"
        };
        return Task.FromResult(result);
    }
}
