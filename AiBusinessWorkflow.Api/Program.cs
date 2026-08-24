using System.ClientModel;
using OpenAI.Responses;
using AiBusinessWorkflow.Api.Data;
using AiBusinessWorkflow.Api.Middleware;
using AiBusinessWorkflow.Api.Services.AI;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/api/health", () => new
{
    status = "ok",
    service = "AiBusinessWorkflow.Api"
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

app.Run();

public partial class Program { }
