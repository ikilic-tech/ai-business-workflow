using System.Net;
using System.Net.Http.Json;
using System.Text;
using AiBusinessWorkflow.Api.Models;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Integration;

public class BusinessIntelligenceEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BusinessIntelligenceEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // Customer Risk Endpoint

    [Fact]
    public async Task CustomerRisk_WithValidRequest_ShouldReturn200()
    {
        var customer = new
        {
            companyName = "Test Corp",
            industry = "Technology",
            employeeCount = 100,
            annualRevenue = 5000000,
            contactName = "John Doe",
            contactEmail = "john@test.com",
            accountAge = "2 years",
            paymentHistory = "Always on time payments",
            activities = new[]
            {
                new { type = "Meeting", date = "2024-01-15", description = "Quarterly review meeting", outcome = "Positive feedback" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", customer);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CustomerRiskAssessment>();
        result.Should().NotBeNull();
        result!.CompanyName.Should().Be("Test Corp");
        result.RiskScore.Should().Be(35);
    }

    [Fact]
    public async Task CustomerRisk_WithMissingCompanyName_ShouldReturn400()
    {
        var customer = new
        {
            companyName = "",
            industry = "Technology",
            employeeCount = 100,
            annualRevenue = 5000000,
            contactName = "John Doe",
            contactEmail = "john@test.com",
            accountAge = "2 years",
            paymentHistory = "Always on time payments",
            activities = new[]
            {
                new { type = "Meeting", date = "2024-01-15", description = "Quarterly review meeting", outcome = "Positive" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", customer);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Activity Summary Endpoint

    [Fact]
    public async Task ActivitySummary_WithValidRequest_ShouldReturn200()
    {
        var request = new
        {
            department = "Sales",
            period = "Q1 2024",
            activities = new[]
            {
                new { employeeName = "Alice Johnson", activityType = "Call", date = "2024-01-08", duration = "30 min", description = "Sales call to prospect company", result = "Meeting booked" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/activity-summary", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ActivitySummaryReport>();
        result.Should().NotBeNull();
        result!.Department.Should().Be("Sales");
    }

    [Fact]
    public async Task ActivitySummary_WithEmptyDepartment_ShouldReturn400()
    {
        var request = new
        {
            department = "",
            period = "Q1 2024",
            activities = new[]
            {
                new { employeeName = "Alice", activityType = "Call", date = "2024-01-08", duration = "30 min", description = "Sales call to prospect", result = "Booked" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/activity-summary", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Opportunity Analysis Endpoint

    [Fact]
    public async Task OpportunityAnalysis_WithValidRequest_ShouldReturn200()
    {
        var opportunity = new
        {
            accountName = "Test Account",
            dealValue = 50000,
            stage = "Proposal",
            expectedCloseDate = "2024-06-30",
            competitorInfo = "Main competitor details here",
            notes = "Notes about the opportunity",
            activities = new[]
            {
                new { type = "Demo", date = "2024-01-15", description = "Product demonstration for team", contactPerson = "Jane Smith" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/opportunity-analysis", opportunity);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<OpportunityAnalysisResult>();
        result.Should().NotBeNull();
        result!.AccountName.Should().Be("Test Account");
        result.WinProbability.Should().Be(70);
    }

    [Fact]
    public async Task OpportunityAnalysis_WithMissingAccountName_ShouldReturn400()
    {
        var opportunity = new
        {
            accountName = "",
            dealValue = 50000,
            stage = "Proposal",
            expectedCloseDate = "2024-06-30",
            competitorInfo = "Competitor info here",
            notes = "Notes about opportunity",
            activities = new[]
            {
                new { type = "Demo", date = "2024-01-15", description = "Product demo for team", contactPerson = "Jane Smith" }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/opportunity-analysis", opportunity);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Recommended Actions Endpoint

    [Fact]
    public async Task RecommendedActions_WithValidRequest_ShouldReturn200()
    {
        var request = new
        {
            businessArea = "Sales Operations",
            currentChallenges = "Sales cycle has lengthened significantly over the past quarter",
            availableResources = "12 sales reps, CRM platform, training budget",
            goals = "Reduce sales cycle, improve win rate",
            recentMetrics = "Q4 2023: Revenue $2.1M against target of $2.5M"
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/recommended-actions", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RecommendedActionsReport>();
        result.Should().NotBeNull();
        result!.BusinessArea.Should().Be("Sales Operations");
    }

    [Fact]
    public async Task RecommendedActions_WithEmptyBusinessArea_ShouldReturn400()
    {
        var request = new
        {
            businessArea = "",
            currentChallenges = "Some challenges that are at least 10 chars",
            availableResources = "Some resources",
            goals = "Some goals here",
            recentMetrics = "Some metrics that are at least 10 chars"
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/recommended-actions", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // Dashboard Endpoint

    [Fact]
    public async Task Dashboard_WithCustomerOnly_ShouldReturn200WithPartialResults()
    {
        var request = new
        {
            customer = new
            {
                companyName = "Test Corp",
                industry = "Technology",
                employeeCount = 100,
                annualRevenue = 5000000,
                contactName = "John Doe",
                contactEmail = "john@test.com",
                accountAge = "2 years",
                paymentHistory = "Always on time payments",
                activities = new[]
                {
                    new { type = "Meeting", date = "2024-01-15", description = "Quarterly review meeting", outcome = "Positive feedback" }
                }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/dashboard", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DashboardSummary>();
        result.Should().NotBeNull();
        result!.CustomerRisk.Should().NotBeNull();
        result.ActivitySummary.Should().BeNull();
        result.OpportunityAnalysis.Should().BeNull();
        result.RecommendedActions.Should().BeNull();
    }

    [Fact]
    public async Task Dashboard_WithEmptyRequest_ShouldReturn400()
    {
        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/intelligence/dashboard", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Dashboard_WithAllInputs_ShouldReturn200WithAllResults()
    {
        var request = new
        {
            customer = new
            {
                companyName = "Test Corp",
                industry = "Technology",
                employeeCount = 100,
                annualRevenue = 5000000,
                contactName = "John Doe",
                contactEmail = "john@test.com",
                accountAge = "2 years",
                paymentHistory = "Always on time payments",
                activities = new[]
                {
                    new { type = "Meeting", date = "2024-01-15", description = "Quarterly review meeting", outcome = "Positive feedback" }
                }
            },
            opportunity = new
            {
                accountName = "Test Account",
                dealValue = 50000,
                stage = "Proposal",
                expectedCloseDate = "2024-06-30",
                competitorInfo = "Main competitor details here",
                notes = "Notes about the opportunity",
                activities = new[]
                {
                    new { type = "Demo", date = "2024-01-15", description = "Product demonstration for team", contactPerson = "Jane Smith" }
                }
            },
            activities = new
            {
                department = "Sales",
                period = "Q1 2024",
                activities = new[]
                {
                    new { employeeName = "Alice Johnson", activityType = "Call", date = "2024-01-08", duration = "30 min", description = "Sales call to prospect company", result = "Meeting booked" }
                }
            },
            actionsContext = new
            {
                businessArea = "Sales Operations",
                currentChallenges = "Sales cycle has lengthened significantly over the past quarter",
                availableResources = "12 sales reps, CRM platform, training budget",
                goals = "Reduce sales cycle, improve win rate",
                recentMetrics = "Q4 2023: Revenue $2.1M against target of $2.5M"
            }
        };

        var response = await _client.PostAsJsonAsync("/api/intelligence/dashboard", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<DashboardSummary>();
        result.Should().NotBeNull();
        result!.CustomerRisk.Should().NotBeNull();
        result.ActivitySummary.Should().NotBeNull();
        result.OpportunityAnalysis.Should().NotBeNull();
        result.RecommendedActions.Should().NotBeNull();
    }

    // Sample Data Endpoints

    [Fact]
    public async Task SampleCustomers_ShouldReturn200WithList()
    {
        var response = await _client.GetAsync("/api/samples/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customers = await response.Content.ReadFromJsonAsync<List<CustomerProfile>>();
        customers.Should().NotBeNull();
        customers.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SampleCustomers_WithValidIndex_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples/customers/0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerProfile>();
        customer.Should().NotBeNull();
        customer!.CompanyName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SampleCustomers_WithInvalidIndex_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/samples/customers/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SampleOpportunities_ShouldReturn200WithList()
    {
        var response = await _client.GetAsync("/api/samples/opportunities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var opportunities = await response.Content.ReadFromJsonAsync<List<Opportunity>>();
        opportunities.Should().NotBeNull();
        opportunities.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SampleOpportunities_WithValidIndex_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples/opportunities/0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SampleOpportunities_WithInvalidIndex_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/samples/opportunities/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SampleActivities_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples/activities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<ActivitySummaryRequest>();
        summary.Should().NotBeNull();
        summary!.Department.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SampleActionsContext_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples/actions-context");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var context = await response.Content.ReadFromJsonAsync<RecommendedActionsRequest>();
        context.Should().NotBeNull();
        context!.BusinessArea.Should().NotBeNullOrEmpty();
    }
}
