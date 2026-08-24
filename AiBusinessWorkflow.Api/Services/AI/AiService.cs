using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using OpenAI.Responses;

namespace AiBusinessWorkflow.Api.Services.AI;

public class AiService : IAiService
{
    private readonly ResponsesClient _responsesClient;
    private readonly string _model;
    private readonly ILogger<AiService> _logger;

    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

    public async Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process)
    {
        _logger.LogInformation("Analyzing business process {ProcessId}: {ProcessName}", process.Id, process.Name);

        try
        {
            var prompt = $$"""
                Analyze the following business process and return your analysis as a JSON object.

                Name: {{process.Name}}
                Description: {{process.Description}}
                Input Data: {{process.InputData}}
                Goal: {{process.Goal}}

                Return ONLY a valid JSON object with this exact schema (no markdown, no code fences):
                {
                  "processId": "{{process.Id}}",
                  "processName": "{{process.Name}}",
                  "efficiency": {
                    "score": <number 0-100>,
                    "rating": "<Low|Medium|High|Very High>",
                    "explanation": "<string>"
                  },
                  "bottlenecks": [
                    {
                      "area": "<string>",
                      "severity": "<Low|Medium|High|Critical>",
                      "description": "<string>",
                      "suggestedFix": "<string>"
                    }
                  ],
                  "recommendations": [
                    {
                      "title": "<string>",
                      "priority": "<Low|Medium|High|Critical>",
                      "impact": "<Low|Medium|High>",
                      "effort": "<Low|Medium|High>",
                      "description": "<string>"
                    }
                  ],
                  "automationOpportunities": [
                    {
                      "process": "<string>",
                      "currentState": "<string>",
                      "proposedAutomation": "<string>",
                      "estimatedTimeSaving": "<string>"
                    }
                  ],
                  "overallRiskLevel": "<Low|Medium|High|Critical>",
                  "summary": "<string>"
                }
                """;

            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var rawText = response.Value.GetOutputText();

            _logger.LogInformation("Business process analysis completed for {ProcessId}", process.Id);

            return ParseAnalysisResponse(rawText, process.Id, process.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Business process analysis failed for {ProcessId}", process.Id);
            throw;
        }
    }

    internal static BusinessProcessAnalysis ParseAnalysisResponse(string rawText, string processId, string processName)
    {
        // Strip markdown code fences if present
        var json = rawText.Trim();
        if (json.StartsWith("```"))
        {
            var firstNewline = json.IndexOf('\n');
            if (firstNewline >= 0)
                json = json[(firstNewline + 1)..];
            if (json.EndsWith("```"))
                json = json[..^3];
            json = json.Trim();
        }

        var analysis = JsonSerializer.Deserialize<BusinessProcessAnalysis>(json, JsonOptions);

        if (analysis is null)
            throw new InvalidOperationException("Failed to deserialize AI response into BusinessProcessAnalysis.");

        // Ensure processId and processName are set correctly
        analysis.ProcessId = processId;
        analysis.ProcessName = processName;

        return analysis;
    }
}
