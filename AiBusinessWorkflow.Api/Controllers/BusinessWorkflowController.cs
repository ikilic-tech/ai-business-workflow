using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace AiBusinessWorkflow.Api.Controllers;

[ApiController]
[Route("api/business-workflow")]
public class BusinessWorkflowController : ControllerBase
{
    private readonly IAiService _aiService;

    public BusinessWorkflowController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<BusinessProcessAnalysis>> Analyze(BusinessProcess process, CancellationToken cancellationToken)
    {
        var result = await _aiService.AnalyzeBusinessProcessAsync(process, cancellationToken);
        return Ok(result);
    }
}
