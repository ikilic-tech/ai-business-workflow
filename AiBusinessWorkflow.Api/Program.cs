using System.ClientModel;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using OpenAI.Responses;
using AiBusinessWorkflow.Api.Endpoints;
using AiBusinessWorkflow.Api.HealthChecks;
using AiBusinessWorkflow.Api.Middleware;
using AiBusinessWorkflow.Api.Services.AI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5_242_880; // 5 MB
});

builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var apiKey = builder.Configuration["AI:ApiKey"];
if (!string.IsNullOrWhiteSpace(apiKey))
{
    builder.Services.AddSingleton(new ResponsesClient(new ApiKeyCredential(apiKey)));
}
else
{
    builder.Services.AddSingleton<ResponsesClient>(_ =>
        throw new InvalidOperationException(
            "AI:ApiKey is not configured. Set it in appsettings.Local.json."));
}
builder.Services.AddSingleton<AiCallMetrics>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<IAiService>(sp =>
    new MeteredAiService(sp.GetRequiredService<AiService>(), sp.GetRequiredService<AiCallMetrics>()));

var defaultPermitLimit = builder.Configuration.GetValue<int>("RateLimiting:Default:PermitLimit");
var defaultWindowSeconds = builder.Configuration.GetValue<int>("RateLimiting:Default:WindowSeconds");
var aiPermitLimit = builder.Configuration.GetValue<int>("RateLimiting:AI:PermitLimit");
var aiWindowSeconds = builder.Configuration.GetValue<int>("RateLimiting:AI:WindowSeconds");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.ContentType = "application/problem+json";
        await Results.Problem(
            statusCode: StatusCodes.Status429TooManyRequests,
            title: "Too Many Requests",
            detail: "Rate limit exceeded. Please try again later.")
            .ExecuteAsync(context.HttpContext);
    };

    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = defaultPermitLimit;
        opt.Window = TimeSpan.FromSeconds(defaultWindowSeconds);
        opt.QueueLimit = 0;
    });
    options.AddFixedWindowLimiter("ai", opt =>
    {
        opt.PermitLimit = aiPermitLimit;
        opt.Window = TimeSpan.FromSeconds(aiWindowSeconds);
        opt.QueueLimit = 0;
    });
});

builder.Services.AddHealthChecks()
    .AddCheck<AiHealthCheck>("ai", tags: new[] { "ready" })
    .AddCheck<MemoryHealthCheck>("memory", tags: new[] { "ready" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseRateLimiter();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "0";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'none'";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    context.Response.Headers.Remove("Server");
    await next();
});

app.MapControllers();
app.MapInfrastructureEndpoints();
app.MapSampleEndpoints();

app.Run();

public partial class Program { }
