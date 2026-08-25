using AiBusinessWorkflow.Api.HealthChecks;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace AiBusinessWorkflow.Api.Endpoints;

public static class InfrastructureEndpoints
{
    public static IEndpointRouteBuilder MapInfrastructureEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/api/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        });

        app.MapGet("/api/ai/test", async (IAiService aiService) =>
        {
            var result = await aiService.TestAiAsync();
            return Results.Ok(new { status = "success", response = result });
        });

        app.MapGet("/api/ai/metrics", (AiCallMetrics metrics) =>
        {
            return Results.Ok(metrics.GetSummary());
        });

        app.MapPost("/api/ai/metrics/reset", (AiCallMetrics metrics) =>
        {
            metrics.Reset();
            return Results.Ok(new { status = "reset" });
        });

        return app;
    }
}
