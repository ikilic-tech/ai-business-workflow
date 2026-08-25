using AiBusinessWorkflow.Api.Controllers;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AiBusinessWorkflow.Tests.Unit.Controllers;

public class BusinessWorkflowControllerTests
{
    private readonly Mock<IAiService> _mockAiService;
    private readonly BusinessWorkflowController _controller;

    public BusinessWorkflowControllerTests()
    {
        _mockAiService = new Mock<IAiService>();
        _controller = new BusinessWorkflowController(_mockAiService.Object);
    }

    private static BusinessProcess CreateValidProcess() => new()
    {
        Id = "test-001",
        Name = "Test Process",
        Description = "A valid test process",
        InputData = "Some input data",
        Goal = "Some goal"
    };

    private static BusinessProcessAnalysis CreateSampleAnalysis() => new()
    {
        ProcessId = "test-001",
        ProcessName = "Test Process",
        Efficiency = new EfficiencyAnalysis { Score = 75, Rating = "Medium", Explanation = "OK" },
        Bottlenecks = new List<Bottleneck>(),
        Recommendations = new List<Recommendation>(),
        AutomationOpportunities = new List<AutomationOpportunity>(),
        OverallRiskLevel = "Low",
        Summary = "Good process"
    };

    [Fact]
    public async Task Analyze_WithValidProcess_ShouldReturnOkWithAnalysis()
    {
        var process = CreateValidProcess();
        var analysis = CreateSampleAnalysis();
        _mockAiService.Setup(s => s.AnalyzeBusinessProcessAsync(It.IsAny<BusinessProcess>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analysis);

        var result = await _controller.Analyze(process, CancellationToken.None);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedAnalysis = okResult.Value.Should().BeOfType<BusinessProcessAnalysis>().Subject;
        returnedAnalysis.ProcessId.Should().Be("test-001");
        returnedAnalysis.Efficiency.Score.Should().Be(75);
    }

    [Fact]
    public async Task Analyze_WhenServiceThrows_ShouldPropagateException()
    {
        var process = CreateValidProcess();
        _mockAiService.Setup(s => s.AnalyzeBusinessProcessAsync(It.IsAny<BusinessProcess>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("AI service failed"));

        var act = () => _controller.Analyze(process, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("AI service failed");
    }

    [Fact]
    public async Task Analyze_ShouldReturnCorrectType()
    {
        var process = CreateValidProcess();
        var analysis = CreateSampleAnalysis();
        _mockAiService.Setup(s => s.AnalyzeBusinessProcessAsync(It.IsAny<BusinessProcess>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analysis);

        var result = await _controller.Analyze(process, CancellationToken.None);

        result.Should().BeOfType<ActionResult<BusinessProcessAnalysis>>();
    }

    [Fact]
    public async Task Analyze_ShouldCallServiceWithProvidedProcess()
    {
        var process = CreateValidProcess();
        var analysis = CreateSampleAnalysis();
        _mockAiService.Setup(s => s.AnalyzeBusinessProcessAsync(It.IsAny<BusinessProcess>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(analysis);

        await _controller.Analyze(process, CancellationToken.None);

        _mockAiService.Verify(s => s.AnalyzeBusinessProcessAsync(
            It.Is<BusinessProcess>(p => p.Id == "test-001"), It.IsAny<CancellationToken>()), Times.Once);
    }
}
