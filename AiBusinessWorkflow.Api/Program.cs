using System.ClientModel;
using OpenAI.Responses;
using AiBusinessWorkflow.Api.Data;
using AiBusinessWorkflow.Api.HealthChecks;
using AiBusinessWorkflow.Api.Middleware;
using AiBusinessWorkflow.Api.Services.AI;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteAsync
});

app.MapGet("/api/ai/test", async (IAiService aiService) =>
{
    var result = await aiService.TestAiAsync();
    return Results.Ok(new { status = "success", response = result });
});

app.MapGet("/api/samples", () => SampleDataGenerator.GetAll());

app.MapGet("/api/samples/{index:int}", (int index) =>
{
    var sample = SampleDataGenerator.GetByIndex(index);
    return sample is not null ? Results.Ok(sample) : Results.NotFound();
});

app.MapGet("/api/samples/customers", () => BusinessIntelligenceSampleData.GetAllCustomers());

app.MapGet("/api/samples/customers/{index:int}", (int index) =>
{
    var sample = BusinessIntelligenceSampleData.GetCustomerByIndex(index);
    return sample is not null ? Results.Ok(sample) : Results.NotFound();
});

app.MapGet("/api/samples/opportunities", () => BusinessIntelligenceSampleData.GetAllOpportunities());

app.MapGet("/api/samples/opportunities/{index:int}", (int index) =>
{
    var sample = BusinessIntelligenceSampleData.GetOpportunityByIndex(index);
    return sample is not null ? Results.Ok(sample) : Results.NotFound();
});

app.MapGet("/api/samples/activities", () => BusinessIntelligenceSampleData.GetActivitySummary());

app.MapGet("/api/samples/actions-context", () => BusinessIntelligenceSampleData.GetActionsContext());

app.Run();

public partial class Program { }
