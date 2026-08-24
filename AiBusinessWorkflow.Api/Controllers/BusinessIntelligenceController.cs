using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AiBusinessWorkflow.Api.Controllers;

[ApiController]
[Route("api/intelligence")]
public class BusinessIntelligenceController : ControllerBase
{
    private readonly IAiService _aiService;

    public BusinessIntelligenceController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("customer-risk")]
    public async Task<ActionResult<CustomerRiskAssessment>> AssessCustomerRisk(CustomerProfile customer)
    {
        var result = await _aiService.AssessCustomerRiskAsync(customer);
        return Ok(result);
    }

    [HttpPost("activity-summary")]
    public async Task<ActionResult<ActivitySummaryReport>> SummarizeActivities(ActivitySummaryRequest request)
    {
        var result = await _aiService.SummarizeActivitiesAsync(request);
        return Ok(result);
    }

    [HttpPost("opportunity-analysis")]
    public async Task<ActionResult<OpportunityAnalysisResult>> AnalyzeOpportunity(Opportunity opportunity)
    {
        var result = await _aiService.AnalyzeOpportunityAsync(opportunity);
        return Ok(result);
    }

    [HttpPost("recommended-actions")]
    public async Task<ActionResult<RecommendedActionsReport>> GenerateRecommendedActions(RecommendedActionsRequest request)
    {
        var result = await _aiService.GenerateRecommendedActionsAsync(request);
        return Ok(result);
    }

    [HttpPost("dashboard")]
    public async Task<ActionResult<DashboardSummary>> GenerateDashboard(DashboardRequest request)
    {
        var tasks = new List<Task>();
        Task<CustomerRiskAssessment>? riskTask = null;
        Task<ActivitySummaryReport>? activityTask = null;
        Task<OpportunityAnalysisResult>? opportunityTask = null;
        Task<RecommendedActionsReport>? actionsTask = null;

        if (request.Customer is not null)
        {
            riskTask = _aiService.AssessCustomerRiskAsync(request.Customer);
            tasks.Add(riskTask);
        }

        if (request.Activities is not null)
        {
            activityTask = _aiService.SummarizeActivitiesAsync(request.Activities);
            tasks.Add(activityTask);
        }

        if (request.Opportunity is not null)
        {
            opportunityTask = _aiService.AnalyzeOpportunityAsync(request.Opportunity);
            tasks.Add(opportunityTask);
        }

        if (request.ActionsContext is not null)
        {
            actionsTask = _aiService.GenerateRecommendedActionsAsync(request.ActionsContext);
            tasks.Add(actionsTask);
        }

        if (tasks.Count == 0)
            return BadRequest("At least one analysis input is required.");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        try
        {
            await Task.WhenAll(tasks).WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout, new ProblemDetails
            {
                Status = StatusCodes.Status504GatewayTimeout,
                Title = "Gateway Timeout",
                Detail = "One or more analysis tasks timed out."
            });
        }

        var summary = new DashboardSummary
        {
            GeneratedAt = DateTime.UtcNow,
            CustomerRisk = riskTask?.Result,
            ActivitySummary = activityTask?.Result,
            OpportunityAnalysis = opportunityTask?.Result,
            RecommendedActions = actionsTask?.Result
        };

        return Ok(summary);
    }
}
