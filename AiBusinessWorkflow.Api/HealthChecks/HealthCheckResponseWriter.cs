using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AiBusinessWorkflow.Api.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
        var isDevelopment = env.IsDevelopment();

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                data = isDevelopment ? e.Value.Data : null,
                exception = isDevelopment ? e.Value.Exception?.Message : null
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
