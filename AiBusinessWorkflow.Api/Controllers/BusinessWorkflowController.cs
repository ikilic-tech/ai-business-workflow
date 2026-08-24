using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Mvc;

namespace AiBusinessWorkflow.Api.Controllers;

[ApiController]
[Route("api/business-workflow")]
public class BusinessWorkflowController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILogger<BusinessWorkflowController> _logger;

    public BusinessWorkflowController(IAiService aiService, ILogger<BusinessWorkflowController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<BusinessProcessAnalysis>> Analyze(BusinessProcess process)
    {
        try
        {
            var result = await _aiService.AnalyzeBusinessProcessAsync(process);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze business process {ProcessId}", process.Id);
            return StatusCode(500, new { error = "An error occurred while analyzing the business process." });
        }
    }
}
