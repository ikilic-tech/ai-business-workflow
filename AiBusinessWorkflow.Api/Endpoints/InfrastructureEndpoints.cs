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

        app.MapGet("/api/ai/test", async (IAiService aiService, CancellationToken cancellationToken) =>
        {
            var result = await aiService.TestAiAsync(cancellationToken);
            return Results.Ok(new { status = "success", response = result });
        }).RequireRateLimiting("ai");

        app.MapGet("/api/ai/metrics", (AiCallMetrics metrics) =>
        {
            return Results.Ok(metrics.GetSummary());
        }).RequireRateLimiting("fixed");

        app.MapPost("/api/ai/metrics/reset", (AiCallMetrics metrics) =>
        {
            metrics.Reset();
            return Results.Ok(new { status = "reset" });
        }).RequireRateLimiting("fixed");

        return app;
    }
}
