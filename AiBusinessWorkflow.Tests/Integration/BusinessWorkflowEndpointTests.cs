using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Integration;

public class BusinessWorkflowEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BusinessWorkflowEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Analyze_WithValidRequest_ShouldReturn200()
    {
        var process = new
        {
            name = "Test Process",
            description = "A valid test process description",
            inputData = "Some valid input data",
            goal = "A valid goal statement"
        };

        var response = await _client.PostAsJsonAsync("/api/business-workflow/analyze", process);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Analyze_WithValidRequest_ShouldReturnStructuredAnalysis()
    {
        var process = new
        {
            name = "Test Process",
            description = "A valid test process description",
            inputData = "Some valid input data",
            goal = "A valid goal statement"
        };

        var response = await _client.PostAsJsonAsync("/api/business-workflow/analyze", process);
        var analysis = await response.Content.ReadFromJsonAsync<BusinessProcessAnalysis>();

        analysis.Should().NotBeNull();
        analysis!.ProcessName.Should().Be("Test Process");
        analysis.Efficiency.Score.Should().Be(75);
        analysis.Bottlenecks.Should().NotBeEmpty();
        analysis.Recommendations.Should().NotBeEmpty();
        analysis.AutomationOpportunities.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Analyze_WithMissingName_ShouldReturn400()
    {
        var process = new
        {
            name = "",
            description = "A valid test process description",
            inputData = "Some valid input data",
            goal = "A valid goal statement"
        };

        var response = await _client.PostAsJsonAsync("/api/business-workflow/analyze", process);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Analyze_WithShortDescription_ShouldReturn400()
    {
        var process = new
        {
            name = "Test Process",
            description = "Short",
            inputData = "Some valid input data",
            goal = "A valid goal statement"
        };

        var response = await _client.PostAsJsonAsync("/api/business-workflow/analyze", process);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Analyze_WithEmptyBody_ShouldReturn400()
    {
        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/business-workflow/analyze", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Analyze_WithNoBody_ShouldReturn400()
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/business-workflow/analyze", content);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Samples_ShouldReturn200WithList()
    {
        var response = await _client.GetAsync("/api/samples");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var samples = await response.Content.ReadFromJsonAsync<List<BusinessProcess>>();
        samples.Should().NotBeNull();
        samples.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Samples_WithValidIndex_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/samples/0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sample = await response.Content.ReadFromJsonAsync<BusinessProcess>();
        sample.Should().NotBeNull();
        sample!.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Samples_WithInvalidIndex_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/samples/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AiTest_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/ai/test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("success");
    }
}
