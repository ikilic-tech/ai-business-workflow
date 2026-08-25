using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AiBusinessWorkflow.Api.HealthChecks;

public class AiHealthCheck : IHealthCheck
{
    private readonly IAiService _aiService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiHealthCheck> _logger;

    public AiHealthCheck(
        IAiService aiService,
        IConfiguration configuration,
        ILogger<AiHealthCheck> logger)
    {
        _aiService = aiService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _aiService.TestAiAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                ["configured"] = true
            };

            return HealthCheckResult.Healthy("AI service is reachable.", data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI health check failed");
            return HealthCheckResult.Unhealthy("AI service is not reachable.", ex);
        }
    }
}
