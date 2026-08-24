using AiBusinessWorkflow.Api.Models;
using OpenAI.Responses;

namespace AiBusinessWorkflow.Api.Services.AI;

public class AiService : IAiService
{
    private readonly ResponsesClient _responsesClient;
    private readonly string _model;
    private readonly ILogger<AiService> _logger;

    public AiService(ResponsesClient responsesClient, IConfiguration configuration, ILogger<AiService> logger)
    {
        _responsesClient = responsesClient;
        _model = configuration["AI:Model"] ?? "gpt-4o";
        _logger = logger;
    }

    public async Task<string> TestAiAsync()
    {
        _logger.LogInformation("Testing AI connection with model {Model}", _model);

        try
        {
            var response = await _responsesClient.CreateResponseAsync(_model, "Say hello in one sentence.");
            var result = response.Value.GetOutputText();

            _logger.LogInformation("AI test successful");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI test failed");
            throw;
        }
    }

    public async Task<string> AnalyzeBusinessProcessAsync(BusinessProcess process)
    {
        _logger.LogInformation("Analyzing business process {ProcessId}: {ProcessName}", process.Id, process.Name);

        try
        {
            var prompt = $"""
                Analyze the following business process and provide optimization suggestions:

                Name: {process.Name}
                Description: {process.Description}
                Input Data: {process.InputData}
                Goal: {process.Goal}

                Please provide:
                1. Process efficiency analysis
                2. Potential bottlenecks
                3. Optimization recommendations
                4. Automation opportunities
                """;

            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var result = response.Value.GetOutputText();

            _logger.LogInformation("Business process analysis completed for {ProcessId}", process.Id);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Business process analysis failed for {ProcessId}", process.Id);
            throw;
        }
    }
}
