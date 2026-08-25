using System.Text.Json;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Evaluation;

/// <summary>
/// Validates evaluation dataset files: structure, required fields, and scenario consistency.
/// </summary>
public class EvaluationDatasetValidationTests
{
    private static readonly string EvaluationRoot = FindEvaluationRoot();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    // --- Dataset File Existence ---

    [Theory]
    [InlineData("datasets/customers.json")]
    [InlineData("datasets/opportunities.json")]
    [InlineData("datasets/activities.json")]
    [InlineData("datasets/business-processes.json")]
    [InlineData("datasets/recommended-actions.json")]
    [InlineData("datasets/adversarial.json")]
    [InlineData("scenarios/validation-checks.json")]
    public void DatasetFile_ShouldExist(string relativePath)
    {
        var fullPath = Path.Combine(EvaluationRoot, relativePath);
        File.Exists(fullPath).Should().BeTrue($"evaluation file {relativePath} should exist");
    }

    // --- Dataset JSON Validity ---

    [Theory]
    [InlineData("datasets/customers.json")]
    [InlineData("datasets/opportunities.json")]
    [InlineData("datasets/activities.json")]
    [InlineData("datasets/business-processes.json")]
    [InlineData("datasets/recommended-actions.json")]
    public void DatasetFile_ShouldBeValidJsonArray(string relativePath)
    {
        var json = File.ReadAllText(Path.Combine(EvaluationRoot, relativePath));
        var scenarios = JsonSerializer.Deserialize<JsonElement>(json);

        scenarios.ValueKind.Should().Be(JsonValueKind.Array, $"{relativePath} should be a JSON array");
        scenarios.GetArrayLength().Should().BeGreaterThan(0, $"{relativePath} should have at least one scenario");
    }

    // --- Scenario Structure ---

    [Theory]
    [InlineData("datasets/customers.json")]
    [InlineData("datasets/opportunities.json")]
    [InlineData("datasets/activities.json")]
    [InlineData("datasets/business-processes.json")]
    [InlineData("datasets/recommended-actions.json")]
    public void DatasetScenarios_ShouldHaveRequiredFields(string relativePath)
    {
        var json = File.ReadAllText(Path.Combine(EvaluationRoot, relativePath));
        var scenarios = JsonSerializer.Deserialize<JsonElement>(json);

        foreach (var scenario in scenarios.EnumerateArray())
        {
            scenario.TryGetProperty("id", out _).Should().BeTrue($"scenario in {relativePath} should have 'id'");
            scenario.TryGetProperty("description", out _).Should().BeTrue($"scenario in {relativePath} should have 'description'");
            scenario.TryGetProperty("input", out _).Should().BeTrue($"scenario in {relativePath} should have 'input'");
            scenario.TryGetProperty("expectedBehaviour", out _).Should().BeTrue($"scenario in {relativePath} should have 'expectedBehaviour'");
        }
    }

    [Fact]
    public void DatasetScenarios_ShouldHaveUniqueIds()
    {
        var datasetFiles = new[]
        {
            "datasets/customers.json",
            "datasets/opportunities.json",
            "datasets/activities.json",
            "datasets/business-processes.json",
            "datasets/recommended-actions.json"
        };

        var allIds = new List<string>();

        foreach (var file in datasetFiles)
        {
            var json = File.ReadAllText(Path.Combine(EvaluationRoot, file));
            var scenarios = JsonSerializer.Deserialize<JsonElement>(json);
            foreach (var scenario in scenarios.EnumerateArray())
            {
                allIds.Add(scenario.GetProperty("id").GetString()!);
            }
        }

        allIds.Should().OnlyHaveUniqueItems("all scenario IDs across datasets should be unique");
    }

    // --- Adversarial Dataset ---

    [Fact]
    public void AdversarialDataset_ShouldHaveRequiredFields()
    {
        var json = File.ReadAllText(Path.Combine(EvaluationRoot, "datasets/adversarial.json"));
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        root.TryGetProperty("scenarios", out var scenarios).Should().BeTrue();
        scenarios.GetArrayLength().Should().BeGreaterThan(0);

        foreach (var scenario in scenarios.EnumerateArray())
        {
            scenario.TryGetProperty("id", out _).Should().BeTrue();
            scenario.TryGetProperty("vector", out _).Should().BeTrue();
            scenario.TryGetProperty("payload", out _).Should().BeTrue();
            scenario.TryGetProperty("targetField", out _).Should().BeTrue();
            scenario.TryGetProperty("targetPrompt", out _).Should().BeTrue();
            scenario.TryGetProperty("expectedDefence", out _).Should().BeTrue();
        }
    }

    // --- Validation Checks ---

    [Fact]
    public void ValidationChecks_ShouldHaveRequiredFields()
    {
        var json = File.ReadAllText(Path.Combine(EvaluationRoot, "scenarios/validation-checks.json"));
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        root.TryGetProperty("checks", out var checks).Should().BeTrue();
        checks.GetArrayLength().Should().BeGreaterThan(0);

        foreach (var check in checks.EnumerateArray())
        {
            check.TryGetProperty("id", out _).Should().BeTrue();
            check.TryGetProperty("name", out _).Should().BeTrue();
            check.TryGetProperty("appliesTo", out _).Should().BeTrue();
        }
    }

    // --- Customer Dataset Specific ---

    [Fact]
    public void CustomerDataset_ShouldHaveValidRiskScoreRanges()
    {
        var json = File.ReadAllText(Path.Combine(EvaluationRoot, "datasets/customers.json"));
        var scenarios = JsonSerializer.Deserialize<JsonElement>(json);

        foreach (var scenario in scenarios.EnumerateArray())
        {
            var expected = scenario.GetProperty("expectedBehaviour");
            var range = expected.GetProperty("riskScoreRange");
            var min = range[0].GetInt32();
            var max = range[1].GetInt32();

            min.Should().BeGreaterOrEqualTo(0);
            max.Should().BeLessOrEqualTo(100);
            min.Should().BeLessThanOrEqualTo(max);
        }
    }

    // --- Helpers ---

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
}
