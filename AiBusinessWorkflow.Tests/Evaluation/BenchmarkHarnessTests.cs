using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Tests.Integration;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Evaluation;

/// <summary>
/// Benchmark harness that runs evaluation datasets through the API endpoints
/// and validates responses against expected behaviour criteria.
///
/// This harness currently runs against FakeAiService (deterministic).
/// To run against a real AI provider, replace the factory with one that uses
/// the actual AiService and set the AI:ApiKey configuration.
/// </summary>
public class BenchmarkHarnessTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly string EvaluationRoot = FindEvaluationRoot();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public BenchmarkHarnessTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // --- Customer Risk Benchmark ---

    [Fact]
    public async Task CustomerRisk_AllScenarios_ShouldReturnValidResponses()
    {
        var scenarios = LoadScenarios("datasets/customers.json");
        var results = new List<BenchmarkResult>();

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var input = scenario.GetProperty("input");
            var sw = Stopwatch.StartNew();

            var response = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", input);
            sw.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"scenario {scenario.GetProperty("id")} should return 200");

            var result = await response.Content.ReadFromJsonAsync<CustomerRiskAssessment>(JsonOptions);
            result.Should().NotBeNull();
            result!.RiskScore.Should().BeInRange(0, 100);
            result.RiskLevel.Should().NotBeNullOrEmpty();
            result.Summary.Should().NotBeNullOrEmpty();

            results.Add(new BenchmarkResult
            {
                ScenarioId = scenario.GetProperty("id").GetString()!,
                Endpoint = "customer-risk",
                StatusCode = (int)response.StatusCode,
                ElapsedMs = sw.ElapsedMilliseconds,
                ResponseValid = true
            });
        }

        results.Should().AllSatisfy(r => r.ResponseValid.Should().BeTrue());
    }

    // --- Opportunity Analysis Benchmark ---

    [Fact]
    public async Task OpportunityAnalysis_AllScenarios_ShouldReturnValidResponses()
    {
        var scenarios = LoadScenarios("datasets/opportunities.json");
        var results = new List<BenchmarkResult>();

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var input = scenario.GetProperty("input");
            var sw = Stopwatch.StartNew();

            var response = await _client.PostAsJsonAsync("/api/intelligence/opportunity-analysis", input);
            sw.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"scenario {scenario.GetProperty("id")} should return 200");

            var result = await response.Content.ReadFromJsonAsync<OpportunityAnalysisResult>(JsonOptions);
            result.Should().NotBeNull();
            result!.WinProbability.Should().BeInRange(0, 100);
            result.Verdict.Should().NotBeNullOrEmpty();
            result.Summary.Should().NotBeNullOrEmpty();

            results.Add(new BenchmarkResult
            {
                ScenarioId = scenario.GetProperty("id").GetString()!,
                Endpoint = "opportunity-analysis",
                StatusCode = (int)response.StatusCode,
                ElapsedMs = sw.ElapsedMilliseconds,
                ResponseValid = true
            });
        }

        results.Should().AllSatisfy(r => r.ResponseValid.Should().BeTrue());
    }

    // --- Activity Summary Benchmark ---

    [Fact]
    public async Task ActivitySummary_AllScenarios_ShouldReturnValidResponses()
    {
        var scenarios = LoadScenarios("datasets/activities.json");

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var input = scenario.GetProperty("input");

            var response = await _client.PostAsJsonAsync("/api/intelligence/activity-summary", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"scenario {scenario.GetProperty("id")} should return 200");

            var result = await response.Content.ReadFromJsonAsync<ActivitySummaryReport>(JsonOptions);
            result.Should().NotBeNull();
            result!.TotalActivities.Should().BeGreaterOrEqualTo(0);
            result.Summary.Should().NotBeNullOrEmpty();
        }
    }

    // --- Business Process Benchmark ---

    [Fact]
    public async Task BusinessProcess_AllScenarios_ShouldReturnValidResponses()
    {
        var scenarios = LoadScenarios("datasets/business-processes.json");

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var input = scenario.GetProperty("input");

            var response = await _client.PostAsJsonAsync("/api/business-workflow/analyze", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"scenario {scenario.GetProperty("id")} should return 200");

            var result = await response.Content.ReadFromJsonAsync<BusinessProcessAnalysis>(JsonOptions);
            result.Should().NotBeNull();
            result!.Efficiency.Should().NotBeNull();
            result.Efficiency.Score.Should().BeInRange(0, 100);
            result.Summary.Should().NotBeNullOrEmpty();
        }
    }

    // --- Recommended Actions Benchmark ---

    [Fact]
    public async Task RecommendedActions_AllScenarios_ShouldReturnValidResponses()
    {
        var scenarios = LoadScenarios("datasets/recommended-actions.json");

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var input = scenario.GetProperty("input");

            var response = await _client.PostAsJsonAsync("/api/intelligence/recommended-actions", input);

            response.StatusCode.Should().Be(HttpStatusCode.OK,
                $"scenario {scenario.GetProperty("id")} should return 200");

            var result = await response.Content.ReadFromJsonAsync<RecommendedActionsReport>(JsonOptions);
            result.Should().NotBeNull();
            result!.Actions.Should().NotBeNull();
            result.Summary.Should().NotBeNullOrEmpty();
        }
    }

    // --- Response Timing ---

    [Fact]
    public async Task AllEndpoints_ShouldRespondWithinAcceptableTime()
    {
        var customer = new
        {
            companyName = "Benchmark Corp",
            industry = "Technology",
            employeeCount = 100,
            annualRevenue = 5000000,
            contactName = "Test",
            contactEmail = "test@benchmark.com",
            accountAge = "2 years",
            paymentHistory = "Always on time",
            activities = new[]
            {
                new { type = "Call", date = "2024-01-01", description = "Test call", outcome = "OK" }
            }
        };

        var sw = Stopwatch.StartNew();
        var response = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", customer);
        sw.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // With FakeAiService, response should be near-instant
        // With real AI, adjust this threshold (e.g., 30000ms)
        sw.ElapsedMilliseconds.Should().BeLessThan(5000,
            "API response should complete within acceptable time");
    }

    // --- Helpers ---

    private static JsonElement LoadScenarios(string relativePath)
    {
        var fullPath = Path.Combine(EvaluationRoot, relativePath);
        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<JsonElement>(json);
    }

    private static string FindEvaluationRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, "evaluation");
            if (Directory.Exists(candidate) && File.Exists(Path.Combine(candidate, "README.md")))
                return candidate;
            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the evaluation directory. Ensure it exists at the solution root with a README.md.");
    }

    private record BenchmarkResult
    {
        public required string ScenarioId { get; init; }
        public required string Endpoint { get; init; }
        public required int StatusCode { get; init; }
        public required long ElapsedMs { get; init; }
        public required bool ResponseValid { get; init; }
    }
}
