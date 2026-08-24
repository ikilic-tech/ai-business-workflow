using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace AiBusinessWorkflow.Api.Middleware;

public class ApiKeyAuthMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";

    private static readonly string[] PublicPaths =
    {
        "/api/health",
        "/swagger",
        "/api/samples"
    };

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;

    public ApiKeyAuthMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsPublicPath(path))
        {
            await _next(context);
            return;
        }

        var configuredKeys = _configuration.GetSection("Authentication:ApiKeys").Get<string[]>();

        // If no keys are configured, only allow in Development
        if (configuredKeys is null || configuredKeys.Length == 0)
        {
            if (!IsDevEnvironment(context))
            {
                _logger.LogError("No API keys configured in non-Development environment");
                await WriteUnauthorizedResponse(context, "API authentication is not configured.");
                return;
            }

            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey) ||
            string.IsNullOrWhiteSpace(providedKey))
        {
            _logger.LogWarning("Request to {Path} rejected: missing API key", path);
            await WriteUnauthorizedResponse(context, "API key is required. Provide it via the X-Api-Key header.");
            return;
        }

        var providedBytes = Encoding.UTF8.GetBytes(providedKey.ToString());
        var isValid = configuredKeys.Any(key =>
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            return keyBytes.Length == providedBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(keyBytes, providedBytes);
        });

        if (!isValid)
        {
            _logger.LogWarning("Request to {Path} rejected: invalid API key", path);
            await WriteForbiddenResponse(context, "The provided API key is not valid.");
            return;
        }

        await _next(context);
    }

    private static bool IsDevEnvironment(HttpContext context)
    {
        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
        return env.IsDevelopment();
    }

    private static bool IsPublicPath(string path)
    {
        return PublicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Instance = context.Request.Path
        };

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static async Task WriteForbiddenResponse(HttpContext context, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            Instance = context.Request.Path
        };

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
