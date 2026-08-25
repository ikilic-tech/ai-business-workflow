using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

/// <summary>
/// Tests for the deterministic baseline service, verifying rule-based analysis
/// produces consistent, valid results that can serve as a comparison baseline.
/// </summary>
public class DeterministicBaselineTests
{
    private readonly DeterministicBaselineService _service = new();

    // --- Business Process ---

    [Fact]
    public async Task AnalyzeBusinessProcess_ManualProcess_ShouldScoreLow()
    {
        var process = new BusinessProcess
        {
            Name = "Invoice Processing",
            Description = "Invoices are received by email, manually entered into spreadsheet",
            InputData = "Paper invoices",
            Goal = "Reduce time"
        };

        var result = await _service.AnalyzeBusinessProcessAsync(process);

        result.Efficiency.Score.Should().Be(35);
        result.Efficiency.Rating.Should().Be("Low");
        result.Bottlenecks.Should().NotBeEmpty();
        result.AutomationOpportunities.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AnalyzeBusinessProcess_DigitalProcess_ShouldScoreHigh()
    {
        var process = new BusinessProcess
        {
            Name = "Digital Onboarding",
            Description = "Fully automated digital process with API integration",
            InputData = "Online form",
            Goal = "Maintain speed"
        };

        var result = await _service.AnalyzeBusinessProcessAsync(process);

        result.Efficiency.Score.Should().Be(75);
        result.Efficiency.Rating.Should().Be("High");
    }

    // --- Customer Risk ---

    [Fact]
    public async Task AssessCustomerRisk_GoodHistory_ShouldBeLowRisk()
    {
        var customer = new CustomerProfile
        {
            CompanyName = "Good Corp",
            Industry = "Tech",
            EmployeeCount = 100,
            AnnualRevenue = 5000000,
            ContactName = "Test",
            ContactEmail = "test@test.com",
            AccountAge = "2 years",
            PaymentHistory = "Always on time, consistent payments",
            Activities = new List<CustomerActivity>
            {
                new() { Type = "Call", Date = "2024-01-01", Description = "Check-in", Outcome = "Positive" },
                new() { Type = "Meeting", Date = "2024-02-01", Description = "Review", Outcome = "Positive" },
                new() { Type = "Call", Date = "2024-03-01", Description = "Follow-up", Outcome = "Positive" }
            }
        };

        var result = await _service.AssessCustomerRiskAsync(customer);

        result.RiskScore.Should().BeLessThan(30);
        result.RiskLevel.Should().Be("Low");
        result.EngagementTrend.Should().Be("Increasing");
    }

    [Fact]
    public async Task AssessCustomerRisk_MissedPayments_ShouldBeHighRisk()
    {
        var customer = new CustomerProfile
        {
            CompanyName = "At Risk Corp",
            Industry = "Retail",
            EmployeeCount = 50,
            AnnualRevenue = 2000000,
            ContactName = "Test",
            ContactEmail = "test@test.com",
            AccountAge = "1 year",
            PaymentHistory = "Three missed payments, currently overdue",
            Activities = new List<CustomerActivity>()
        };

        var result = await _service.AssessCustomerRiskAsync(customer);

        result.RiskScore.Should().BeGreaterOrEqualTo(70);
        result.RiskLevel.Should().Be("High");
        result.EngagementTrend.Should().Be("Declining");
    }

    // --- Activity Summary ---

    [Fact]
    public async Task SummarizeActivities_ShouldCountCorrectly()
    {
        var request = new ActivitySummaryRequest
        {
            Department = "Sales",
            Period = "Q1 2024",
            Activities = new List<ActivityEntry>
            {
                new() { EmployeeName = "Alice", ActivityType = "Call", Date = "2024-01-01", Duration = "30min", Description = "Sales call", Result = "OK" },
                new() { EmployeeName = "Bob", ActivityType = "Meeting", Date = "2024-01-02", Duration = "1h", Description = "Client meeting", Result = "Good" },
                new() { EmployeeName = "Alice", ActivityType = "Call", Date = "2024-01-03", Duration = "20min", Description = "Follow-up call", Result = "OK" }
            }
        };

        var result = await _service.SummarizeActivitiesAsync(request);

        result.TotalActivities.Should().Be(3);
        result.UniqueEmployees.Should().Be(2);
        result.CategoryBreakdown.Should().HaveCount(2);
        result.CategoryBreakdown.First(c => c.Category == "Call").Count.Should().Be(2);
    }

    // --- Opportunity Analysis ---

    [Fact]
    public async Task AnalyzeOpportunity_WithChampion_ShouldBePositive()
    {
        var opportunity = new Opportunity
        {
            AccountName = "Strong Account",
            DealValue = 100000,
            Stage = "Proposal",
            ExpectedCloseDate = "2024-06-30",
            CompetitorInfo = "Weak competitor",
            Notes = "CTO is our champion. Budget approved and committed.",
            Activities = new List<OpportunityActivity>
            {
                new() { Type = "Demo", Date = "2024-01-01", Description = "Full demo", ContactPerson = "CTO" },
                new() { Type = "Meeting", Date = "2024-02-01", Description = "Deep dive", ContactPerson = "VP" },
                new() { Type = "Proposal", Date = "2024-03-01", Description = "Sent proposal", ContactPerson = "CTO" }
            }
        };

        var result = await _service.AnalyzeOpportunityAsync(opportunity);

        result.WinProbability.Should().BeGreaterOrEqualTo(65);
        result.Verdict.Should().BeOneOf("Likely Win", "Toss-Up");
    }

    [Fact]
    public async Task AnalyzeOpportunity_Stalled_ShouldBeNegative()
    {
        var opportunity = new Opportunity
        {
            AccountName = "Stalled Account",
            DealValue = 50000,
            Stage = "Negotiation",
            ExpectedCloseDate = "2024-04-30",
            CompetitorInfo = "Aggressive competitor",
            Notes = "Contact has gone quiet. Email unanswered. Budget may have been reallocated.",
            Activities = new List<OpportunityActivity>
            {
                new() { Type = "Call", Date = "2024-01-01", Description = "Voicemail", ContactPerson = "PM" }
            }
        };

        var result = await _service.AnalyzeOpportunityAsync(opportunity);

        result.WinProbability.Should().BeLessThan(40);
        result.Verdict.Should().BeOneOf("At Risk", "Likely Loss");
    }

    // --- Recommended Actions ---

    [Fact]
    public async Task GenerateRecommendedActions_ShouldReturnAtLeastTwoActions()
    {
        var request = new RecommendedActionsRequest
        {
            BusinessArea = "Sales",
            CurrentChallenges = "Sales cycle too long",
            AvailableResources = "10 reps",
            Goals = "Reduce cycle time",
            RecentMetrics = "Revenue below target"
        };

        var result = await _service.GenerateRecommendedActionsAsync(request);

        result.Actions.Should().HaveCountGreaterOrEqualTo(2);
        result.QuickWins.Should().NotBeEmpty();
        result.LongTermInitiatives.Should().NotBeEmpty();
        result.Summary.Should().NotBeNullOrEmpty();
    }

    // --- Determinism Verification ---

    [Fact]
    public async Task AllMethods_ShouldProduceDeterministicResults()
    {
        var customer = new CustomerProfile
        {
            CompanyName = "Deterministic Test",
            Industry = "Tech",
            EmployeeCount = 50,
            AnnualRevenue = 1000000,
            ContactName = "Test",
            ContactEmail = "t@t.com",
            AccountAge = "1 year",
            PaymentHistory = "On time payments",
            Activities = new List<CustomerActivity>
            {
                new() { Type = "Call", Date = "2024-01-01", Description = "Check-in call", Outcome = "OK" }
            }
        };

        var result1 = await _service.AssessCustomerRiskAsync(customer);
        var result2 = await _service.AssessCustomerRiskAsync(customer);

        result1.RiskScore.Should().Be(result2.RiskScore);
        result1.RiskLevel.Should().Be(result2.RiskLevel);
    }
}
