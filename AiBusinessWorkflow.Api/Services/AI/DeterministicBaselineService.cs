using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

/// <summary>
/// Rule-based implementation of IAiService that produces deterministic results
/// without calling an AI provider. Used as a baseline for comparing AI output
/// quality, latency, and reliability.
/// </summary>
public sealed class DeterministicBaselineService : IAiService
{
    public Task<string> TestAiAsync()
    {
        return Task.FromResult("Deterministic baseline service — no AI provider configured.");
    }

    public Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process)
    {
        var hasManualKeyword = ContainsAny(process.Description, "manual", "paper", "email", "spreadsheet");
        var hasDigitalKeyword = ContainsAny(process.Description, "automated", "digital", "api", "self-service");

        var score = hasDigitalKeyword ? 75 : hasManualKeyword ? 35 : 50;

        var analysis = new BusinessProcessAnalysis
        {
            ProcessId = process.Id,
            ProcessName = process.Name,
            Efficiency = new EfficiencyAnalysis
            {
                Score = score,
                Rating = score >= 70 ? "High" : score >= 40 ? "Medium" : "Low",
                Explanation = $"Baseline analysis of '{process.Name}'."
            },
            Bottlenecks = hasManualKeyword
                ? new List<Bottleneck>
                {
                    new()
                    {
                        Area = "Manual processing",
                        Severity = "Medium",
                        Description = "Process contains manual steps.",
                        SuggestedFix = "Consider automation."
                    }
                }
                : new List<Bottleneck>(),
            Recommendations = new List<Recommendation>
            {
                new()
                {
                    Title = "Review process efficiency",
                    Priority = "Medium",
                    Impact = "Medium",
                    Effort = "Low",
                    Description = "Baseline recommendation."
                }
            },
            AutomationOpportunities = hasManualKeyword
                ? new List<AutomationOpportunity>
                {
                    new()
                    {
                        Process = process.Name,
                        CurrentState = "Manual",
                        ProposedAutomation = "Automation candidate identified.",
                        EstimatedTimeSaving = "Unknown"
                    }
                }
                : new List<AutomationOpportunity>(),
            OverallRiskLevel = score < 40 ? "High" : "Medium",
            Summary = $"Deterministic baseline analysis for '{process.Name}' with efficiency score {score}."
        };

        return Task.FromResult(analysis);
    }

    public Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer)
    {
        var riskScore = 50;

        // Simple heuristics
        if (ContainsAny(customer.PaymentHistory, "missed", "overdue", "late"))
            riskScore += 25;
        if (ContainsAny(customer.PaymentHistory, "on time", "consistent", "always"))
            riskScore -= 25;
        if (customer.Activities.Count == 0)
            riskScore += 15;
        if (customer.Activities.Count >= 3)
            riskScore -= 10;

        riskScore = Math.Clamp(riskScore, 0, 100);

        var result = new CustomerRiskAssessment
        {
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            RiskScore = riskScore,
            RiskLevel = riskScore >= 70 ? "High" : riskScore >= 40 ? "Medium" : "Low",
            ChurnProbability = riskScore >= 60 ? "High" : riskScore >= 30 ? "Medium" : "Low",
            EngagementTrend = customer.Activities.Count >= 3 ? "Increasing" : customer.Activities.Count == 0 ? "Declining" : "Stable",
            RiskFactors = riskScore >= 50
                ? new List<RiskFactor>
                {
                    new()
                    {
                        Factor = "Baseline risk factor",
                        Severity = riskScore >= 70 ? "High" : "Medium",
                        Description = "Identified by deterministic rules.",
                        Impact = "Requires attention"
                    }
                }
                : new List<RiskFactor>(),
            RecommendedActions = new List<string> { "Review customer engagement" },
            Summary = $"Baseline risk assessment for '{customer.CompanyName}' with score {riskScore}."
        };

        return Task.FromResult(result);
    }

    public Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request)
    {
        var activities = request.Activities;
        var categoryGroups = activities.GroupBy(a => a.ActivityType).ToList();

        var result = new ActivitySummaryReport
        {
            Department = request.Department,
            Period = request.Period,
            TotalActivities = activities.Count,
            UniqueEmployees = activities.Select(a => a.EmployeeName).Distinct().Count(),
            KeyFindings = new List<string>
            {
                $"{activities.Count} activities recorded in {request.Period}.",
                $"{categoryGroups.Count} activity types identified."
            },
            CategoryBreakdown = categoryGroups.Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = Math.Round(g.Count() / (double)activities.Count * 100, 1)
            }).ToList(),
            Trends = new List<TrendItem>
            {
                new()
                {
                    Indicator = "Activity Volume",
                    Direction = "Stable",
                    Description = "Baseline trend — no historical comparison available."
                }
            },
            Summary = $"Baseline activity summary for {request.Department}, {request.Period}."
        };

        return Task.FromResult(result);
    }

    public Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity)
    {
        var winProb = 50;

        if (opportunity.Activities.Count >= 3) winProb += 15;
        if (opportunity.Activities.Count <= 1) winProb -= 15;
        if (ContainsAny(opportunity.Notes, "quiet", "unanswered", "stalled", "reallocated"))
            winProb -= 20;
        if (ContainsAny(opportunity.Notes, "champion", "committed", "approved"))
            winProb += 20;

        winProb = Math.Clamp(winProb, 0, 100);

        var verdict = winProb >= 70 ? "Likely Win" : winProb >= 50 ? "Toss-Up" : winProb >= 30 ? "At Risk" : "Likely Loss";

        var result = new OpportunityAnalysisResult
        {
            OpportunityId = opportunity.OpportunityId,
            AccountName = opportunity.AccountName,
            WinProbability = winProb,
            Verdict = verdict,
            Strengths = new List<string> { "Baseline analysis" },
            Weaknesses = new List<string> { "No AI-powered insight" },
            CompetitivePosition = "Unknown",
            RecommendedStrategy = new List<StrategyItem>
            {
                new() { Action = "Follow up", Priority = "Medium", Rationale = "Baseline recommendation" }
            },
            NextSteps = new List<string> { "Review opportunity details" },
            Summary = $"Baseline opportunity analysis for '{opportunity.AccountName}' with {winProb}% win probability."
        };

        return Task.FromResult(result);
    }

    public Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request)
    {
        var result = new RecommendedActionsReport
        {
            BusinessArea = request.BusinessArea,
            Actions = new List<ActionItem>
            {
                new()
                {
                    Title = $"Review {request.BusinessArea} performance",
                    Priority = "Medium",
                    Impact = "Medium",
                    Effort = "Low",
                    Description = "Baseline recommendation based on provided context.",
                    ExpectedOutcome = "Improved awareness of current state."
                },
                new()
                {
                    Title = "Address current challenges",
                    Priority = "High",
                    Impact = "High",
                    Effort = "Medium",
                    Description = "Prioritize the challenges outlined in the request.",
                    ExpectedOutcome = "Reduced operational friction."
                }
            },
            QuickWins = new List<string> { "Review recent metrics and identify trends" },
            LongTermInitiatives = new List<string> { "Develop strategic improvement plan" },
            Summary = $"Baseline recommended actions for {request.BusinessArea}."
        };

        return Task.FromResult(result);
    }

    private static bool ContainsAny(string text, params string[] keywords)
    {
        return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}
