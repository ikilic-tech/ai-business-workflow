using AiBusinessWorkflow.Api.Controllers;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AiBusinessWorkflow.Tests.Unit.Controllers;

public class BusinessIntelligenceControllerTests
{
    private readonly Mock<IAiService> _mockAiService;
    private readonly BusinessIntelligenceController _controller;

    public BusinessIntelligenceControllerTests()
    {
        _mockAiService = new Mock<IAiService>();
        _controller = new BusinessIntelligenceController(_mockAiService.Object);
    }

    private static CustomerProfile CreateValidCustomer() => new()
    {
        CustomerId = "cust-001",
        CompanyName = "Test Corp",
        Industry = "Technology",
        EmployeeCount = 100,
        AnnualRevenue = 5000000m,
        ContactName = "John Doe",
        ContactEmail = "john@test.com",
        AccountAge = "2 years",
        PaymentHistory = "Always on time",
        Activities = new List<CustomerActivity>
        {
            new() { Type = "Meeting", Date = "2024-01-15", Description = "Quarterly review meeting", Outcome = "Positive" }
        }
    };

    private static CustomerRiskAssessment CreateSampleRiskAssessment() => new()
    {
        CustomerId = "cust-001",
        CompanyName = "Test Corp",
        RiskScore = 35,
        RiskLevel = "Low",
        ChurnProbability = "Low",
        EngagementTrend = "Stable",
        RiskFactors = new List<RiskFactor>(),
        RecommendedActions = new List<string> { "Continue monitoring" },
        Summary = "Low risk customer"
    };

    private static ActivitySummaryRequest CreateValidActivityRequest() => new()
    {
        Department = "Sales",
        Period = "Q1 2024",
        Activities = new List<ActivityEntry>
        {
            new() { EmployeeName = "Alice", ActivityType = "Call", Date = "2024-01-08", Duration = "30 min", Description = "Sales call to prospect", Result = "Meeting booked" }
        }
    };

    private static ActivitySummaryReport CreateSampleActivityReport() => new()
    {
        Department = "Sales",
        Period = "Q1 2024",
        TotalActivities = 10,
        UniqueEmployees = 3,
        KeyFindings = new List<string> { "High activity" },
        CategoryBreakdown = new List<CategoryStat>(),
        Trends = new List<TrendItem>(),
        Summary = "Good quarter"
    };

    private static Opportunity CreateValidOpportunity() => new()
    {
        OpportunityId = "opp-001",
        AccountName = "Test Account",
        DealValue = 50000m,
        Stage = "Proposal",
        ExpectedCloseDate = "2024-06-30",
        CompetitorInfo = "Main competitor analysis",
        Notes = "Notes about the opportunity",
        Activities = new List<OpportunityActivity>
        {
            new() { Type = "Demo", Date = "2024-01-15", Description = "Product demonstration for team", ContactPerson = "Jane Smith" }
        }
    };

    private static OpportunityAnalysisResult CreateSampleOpportunityResult() => new()
    {
        OpportunityId = "opp-001",
        AccountName = "Test Account",
        WinProbability = 70,
        Verdict = "Likely Win",
        Strengths = new List<string> { "Good fit" },
        Weaknesses = new List<string>(),
        CompetitivePosition = "Leading",
        RecommendedStrategy = new List<StrategyItem>(),
        NextSteps = new List<string> { "Follow up" },
        Summary = "Strong opportunity"
    };

    private static RecommendedActionsRequest CreateValidActionsRequest() => new()
    {
        BusinessArea = "Sales Operations",
        CurrentChallenges = "Sales cycle has lengthened significantly",
        AvailableResources = "12 reps, CRM platform",
        Goals = "Reduce cycle time, improve win rate",
        RecentMetrics = "Revenue below target for two quarters"
    };

    private static RecommendedActionsReport CreateSampleActionsReport() => new()
    {
        BusinessArea = "Sales Operations",
        Actions = new List<ActionItem>
        {
            new() { Title = "Automate reporting", Priority = "High", Impact = "High", Effort = "Medium", Description = "Automate reports", ExpectedOutcome = "Save time" }
        },
        QuickWins = new List<string> { "Update templates" },
        LongTermInitiatives = new List<string> { "CRM upgrade" },
        Summary = "Several improvements identified"
    };

    // Customer Risk Tests

    [Fact]
    public async Task AssessCustomerRisk_WithValidCustomer_ShouldReturnOk()
    {
        var customer = CreateValidCustomer();
        var assessment = CreateSampleRiskAssessment();
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        var result = await _controller.AssessCustomerRisk(customer, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<CustomerRiskAssessment>().Subject;
        returned.CustomerId.Should().Be("cust-001");
        returned.RiskScore.Should().Be(35);
    }

    [Fact]
    public async Task AssessCustomerRisk_WhenServiceThrows_ShouldPropagateException()
    {
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var act = () => _controller.AssessCustomerRisk(CreateValidCustomer(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("AI service failed");
    }

    [Fact]
    public async Task AssessCustomerRisk_ShouldReturnCorrectType()
    {
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleRiskAssessment());

        var result = await _controller.AssessCustomerRisk(CreateValidCustomer(), CancellationToken.None);

        result.Should().BeOfType<ActionResult<CustomerRiskAssessment>>();
    }

    // Activity Summary Tests

    [Fact]
    public async Task SummarizeActivities_WithValidRequest_ShouldReturnOk()
    {
        var request = CreateValidActivityRequest();
        var report = CreateSampleActivityReport();
        _mockAiService.Setup(s => s.SummarizeActivitiesAsync(It.IsAny<ActivitySummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var result = await _controller.SummarizeActivities(request, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<ActivitySummaryReport>().Subject;
        returned.Department.Should().Be("Sales");
        returned.TotalActivities.Should().Be(10);
    }

    [Fact]
    public async Task SummarizeActivities_WhenServiceThrows_ShouldPropagateException()
    {
        _mockAiService.Setup(s => s.SummarizeActivitiesAsync(It.IsAny<ActivitySummaryRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var act = () => _controller.SummarizeActivities(CreateValidActivityRequest(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task SummarizeActivities_ShouldReturnCorrectType()
    {
        _mockAiService.Setup(s => s.SummarizeActivitiesAsync(It.IsAny<ActivitySummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleActivityReport());

        var result = await _controller.SummarizeActivities(CreateValidActivityRequest(), CancellationToken.None);

        result.Should().BeOfType<ActionResult<ActivitySummaryReport>>();
    }

    // Opportunity Analysis Tests

    [Fact]
    public async Task AnalyzeOpportunity_WithValidOpportunity_ShouldReturnOk()
    {
        var opportunity = CreateValidOpportunity();
        var analysis = CreateSampleOpportunityResult();
        _mockAiService.Setup(s => s.AnalyzeOpportunityAsync(It.IsAny<Opportunity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analysis);

        var result = await _controller.AnalyzeOpportunity(opportunity, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<OpportunityAnalysisResult>().Subject;
        returned.OpportunityId.Should().Be("opp-001");
        returned.WinProbability.Should().Be(70);
    }

    [Fact]
    public async Task AnalyzeOpportunity_WhenServiceThrows_ShouldPropagateException()
    {
        _mockAiService.Setup(s => s.AnalyzeOpportunityAsync(It.IsAny<Opportunity>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var act = () => _controller.AnalyzeOpportunity(CreateValidOpportunity(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AnalyzeOpportunity_ShouldReturnCorrectType()
    {
        _mockAiService.Setup(s => s.AnalyzeOpportunityAsync(It.IsAny<Opportunity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleOpportunityResult());

        var result = await _controller.AnalyzeOpportunity(CreateValidOpportunity(), CancellationToken.None);

        result.Should().BeOfType<ActionResult<OpportunityAnalysisResult>>();
    }

    // Recommended Actions Tests

    [Fact]
    public async Task GenerateRecommendedActions_WithValidRequest_ShouldReturnOk()
    {
        var request = CreateValidActionsRequest();
        var report = CreateSampleActionsReport();
        _mockAiService.Setup(s => s.GenerateRecommendedActionsAsync(It.IsAny<RecommendedActionsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(report);

        var result = await _controller.GenerateRecommendedActions(request, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returned = okResult.Value.Should().BeOfType<RecommendedActionsReport>().Subject;
        returned.BusinessArea.Should().Be("Sales Operations");
        returned.Actions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GenerateRecommendedActions_WhenServiceThrows_ShouldPropagateException()
    {
        _mockAiService.Setup(s => s.GenerateRecommendedActionsAsync(It.IsAny<RecommendedActionsRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var act = () => _controller.GenerateRecommendedActions(CreateValidActionsRequest(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateRecommendedActions_ShouldReturnCorrectType()
    {
        _mockAiService.Setup(s => s.GenerateRecommendedActionsAsync(It.IsAny<RecommendedActionsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleActionsReport());

        var result = await _controller.GenerateRecommendedActions(CreateValidActionsRequest(), CancellationToken.None);

        result.Should().BeOfType<ActionResult<RecommendedActionsReport>>();
    }

    // Dashboard Tests

    [Fact]
    public async Task Dashboard_WithAllInputs_ShouldReturnOkWithAllResults()
    {
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleRiskAssessment());
        _mockAiService.Setup(s => s.SummarizeActivitiesAsync(It.IsAny<ActivitySummaryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleActivityReport());
        _mockAiService.Setup(s => s.AnalyzeOpportunityAsync(It.IsAny<Opportunity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleOpportunityResult());
        _mockAiService.Setup(s => s.GenerateRecommendedActionsAsync(It.IsAny<RecommendedActionsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleActionsReport());

        var request = new DashboardRequest
        {
            Customer = CreateValidCustomer(),
            Activities = CreateValidActivityRequest(),
            Opportunity = CreateValidOpportunity(),
            ActionsContext = CreateValidActionsRequest()
        };

        var result = await _controller.GenerateDashboard(request, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dashboard = okResult.Value.Should().BeOfType<DashboardSummary>().Subject;
        dashboard.CustomerRisk.Should().NotBeNull();
        dashboard.ActivitySummary.Should().NotBeNull();
        dashboard.OpportunityAnalysis.Should().NotBeNull();
        dashboard.RecommendedActions.Should().NotBeNull();
        dashboard.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Dashboard_WithNoInputs_ShouldReturnBadRequest()
    {
        var request = new DashboardRequest();

        var result = await _controller.GenerateDashboard(request, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Dashboard_ShouldReturnCorrectType()
    {
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleRiskAssessment());

        var request = new DashboardRequest { Customer = CreateValidCustomer() };

        var result = await _controller.GenerateDashboard(request, CancellationToken.None);

        result.Should().BeOfType<ActionResult<DashboardSummary>>();
    }

    [Fact]
    public async Task Dashboard_WithPartialInputs_ShouldReturnOnlyRequestedResults()
    {
        _mockAiService.Setup(s => s.AssessCustomerRiskAsync(It.IsAny<CustomerProfile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleRiskAssessment());

        var request = new DashboardRequest
        {
            Customer = CreateValidCustomer()
        };

        var result = await _controller.GenerateDashboard(request, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var dashboard = okResult.Value.Should().BeOfType<DashboardSummary>().Subject;
        dashboard.CustomerRisk.Should().NotBeNull();
        dashboard.ActivitySummary.Should().BeNull();
        dashboard.OpportunityAnalysis.Should().BeNull();
        dashboard.RecommendedActions.Should().BeNull();
    }
}
