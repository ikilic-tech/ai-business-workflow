using System.ClientModel;
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
builder.Services.AddScoped<IAiService, AiService>();

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
    await next();
});

app.MapControllers();
app.MapInfrastructureEndpoints();
app.MapSampleEndpoints();

app.Run();

public partial class Program { }
