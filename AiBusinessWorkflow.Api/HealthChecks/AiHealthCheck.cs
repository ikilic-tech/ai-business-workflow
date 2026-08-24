using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenAI.Responses;

namespace AiBusinessWorkflow.Api.HealthChecks;

public class AiHealthCheck : IHealthCheck
{
    private readonly ResponsesClient _responsesClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiHealthCheck> _logger;

    public AiHealthCheck(
        ResponsesClient responsesClient,
        IConfiguration configuration,
        ILogger<AiHealthCheck> logger)
    {
        _responsesClient = responsesClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var model = _configuration["AI:Model"] ?? "gpt-4o";
            var response = await _responsesClient.CreateResponseAsync(model, "Reply with OK");

            var data = new Dictionary<string, object>
            {
                ["provider"] = _configuration["AI:Provider"] ?? "OpenAI",
                ["model"] = model
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
