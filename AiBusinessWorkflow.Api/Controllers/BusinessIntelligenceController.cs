using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AiBusinessWorkflow.Api.Controllers;

[ApiController]
[Route("api/intelligence")]
[EnableRateLimiting("ai")]
[Consumes("application/json")]
public class BusinessIntelligenceController : ControllerBase
{
    private readonly IAiService _aiService;

    public BusinessIntelligenceController(IAiService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>Assesses customer churn risk based on profile, payment history, and activity.</summary>
    /// <param name="customer">The customer profile to assess.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("customer-risk")]
    public async Task<ActionResult<CustomerRiskAssessment>> AssessCustomerRisk(CustomerProfile customer, CancellationToken cancellationToken)
    {
        var result = await _aiService.AssessCustomerRiskAsync(customer, cancellationToken);
        return Ok(result);
    }

    /// <summary>Summarizes department activities for a given period with trends.</summary>
    /// <param name="request">The activity summary request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("activity-summary")]
    public async Task<ActionResult<ActivitySummaryReport>> SummarizeActivities(ActivitySummaryRequest request, CancellationToken cancellationToken)
    {
        var result = await _aiService.SummarizeActivitiesAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Analyzes a sales opportunity and predicts win probability.</summary>
    /// <param name="opportunity">The opportunity to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("opportunity-analysis")]
    public async Task<ActionResult<OpportunityAnalysisResult>> AnalyzeOpportunity(Opportunity opportunity, CancellationToken cancellationToken)
    {
        var result = await _aiService.AnalyzeOpportunityAsync(opportunity, cancellationToken);
        return Ok(result);
    }

    /// <summary>Generates prioritized action items based on business context.</summary>
    /// <param name="request">The recommended actions request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("recommended-actions")]
    public async Task<ActionResult<RecommendedActionsReport>> GenerateRecommendedActions(RecommendedActionsRequest request, CancellationToken cancellationToken)
    {
        var result = await _aiService.GenerateRecommendedActionsAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Runs multiple analyses in parallel and returns a combined dashboard.</summary>
    /// <param name="request">The dashboard request containing one or more analysis inputs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("dashboard")]
    public async Task<ActionResult<DashboardSummary>> GenerateDashboard(DashboardRequest request, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        Task<CustomerRiskAssessment>? riskTask = null;
        Task<ActivitySummaryReport>? activityTask = null;
        Task<OpportunityAnalysisResult>? opportunityTask = null;
        Task<RecommendedActionsReport>? actionsTask = null;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(60));

        if (request.Customer is not null)
        {
            riskTask = _aiService.AssessCustomerRiskAsync(request.Customer, cts.Token);
            tasks.Add(riskTask);
        }

        if (request.Activities is not null)
        {
            activityTask = _aiService.SummarizeActivitiesAsync(request.Activities, cts.Token);
            tasks.Add(activityTask);
        }

        if (request.Opportunity is not null)
        {
            opportunityTask = _aiService.AnalyzeOpportunityAsync(request.Opportunity, cts.Token);
            tasks.Add(opportunityTask);
        }

        if (request.ActionsContext is not null)
        {
            actionsTask = _aiService.GenerateRecommendedActionsAsync(request.ActionsContext, cts.Token);
            tasks.Add(actionsTask);
        }

        if (tasks.Count == 0)
            return BadRequest("At least one analysis input is required.");

        try
        {
            await Task.WhenAll(tasks);
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
