using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AiBusinessWorkflow.Api.Controllers;

[ApiController]
[Route("api/business-workflow")]
[EnableRateLimiting("ai")]
[Consumes("application/json")]
public class BusinessWorkflowController : ControllerBase
{
    private readonly IAiService _aiService;

    public BusinessWorkflowController(IAiService aiService)
    {
        _aiService = aiService;
    }

    /// <summary>Analyzes a business process and returns optimization insights.</summary>
    /// <param name="process">The business process to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("analyze")]
    public async Task<ActionResult<BusinessProcessAnalysis>> Analyze(BusinessProcess process, CancellationToken cancellationToken)
    {
        var result = await _aiService.AnalyzeBusinessProcessAsync(process, cancellationToken);
        return Ok(result);
    }
}
