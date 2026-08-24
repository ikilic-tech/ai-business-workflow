using System.ClientModel;
using OpenAI.Responses;
using AiBusinessWorkflow.Api.Services.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    "appsettings.Local.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/api/health", () => new
{
    status = "ok",
    service = "AiBusinessWorkflow.Api"
});

app.MapGet("/api/ai/test", async (IAiService aiService) =>
{
    try
    {
        var result = await aiService.TestAiAsync();
        return Results.Ok(new { status = "success", response = result });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            title: "AI Test Failed",
            statusCode: 500);
    }
});

app.Run();
