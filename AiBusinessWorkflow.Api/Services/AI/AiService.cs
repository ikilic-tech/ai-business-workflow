using System.Text.Json;
using AiBusinessWorkflow.Api.Models;
using AiBusinessWorkflow.Api.Prompts;
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
            var prompt = BusinessWorkflowPrompt.Build(process);
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

    public async Task<CustomerRiskAssessment> AssessCustomerRiskAsync(CustomerProfile customer)
    {
        _logger.LogInformation("Assessing customer risk for {CustomerId}: {CompanyName}", customer.CustomerId, customer.CompanyName);

        try
        {
            var prompt = CustomerRiskPrompt.Build(customer);
            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var rawText = response.Value.GetOutputText();

            _logger.LogInformation("Customer risk assessment completed for {CustomerId}", customer.CustomerId);

            return ParseCustomerRiskResponse(rawText, customer.CustomerId, customer.CompanyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer risk assessment failed for {CustomerId}", customer.CustomerId);
            throw;
        }
    }

    public async Task<ActivitySummaryReport> SummarizeActivitiesAsync(ActivitySummaryRequest request)
    {
        _logger.LogInformation("Summarizing activities for {Department}, period {Period}", request.Department, request.Period);

        try
        {
            var prompt = ActivitySummaryPrompt.Build(request);
            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var rawText = response.Value.GetOutputText();

            _logger.LogInformation("Activity summary completed for {Department}", request.Department);

            return ParseActivitySummaryResponse(rawText, request.Department, request.Period);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Activity summary failed for {Department}", request.Department);
            throw;
        }
    }

    public async Task<OpportunityAnalysisResult> AnalyzeOpportunityAsync(Opportunity opportunity)
    {
        _logger.LogInformation("Analyzing opportunity {OpportunityId}: {AccountName}", opportunity.OpportunityId, opportunity.AccountName);

        try
        {
            var prompt = OpportunityAnalysisPrompt.Build(opportunity);
            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var rawText = response.Value.GetOutputText();

            _logger.LogInformation("Opportunity analysis completed for {OpportunityId}", opportunity.OpportunityId);

            return ParseOpportunityAnalysisResponse(rawText, opportunity.OpportunityId, opportunity.AccountName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Opportunity analysis failed for {OpportunityId}", opportunity.OpportunityId);
            throw;
        }
    }

    public async Task<RecommendedActionsReport> GenerateRecommendedActionsAsync(RecommendedActionsRequest request)
    {
        _logger.LogInformation("Generating recommended actions for {BusinessArea}", request.BusinessArea);

        try
        {
            var prompt = RecommendedActionsPrompt.Build(request);
            var response = await _responsesClient.CreateResponseAsync(_model, prompt);
            var rawText = response.Value.GetOutputText();

            _logger.LogInformation("Recommended actions generated for {BusinessArea}", request.BusinessArea);

            return ParseRecommendedActionsResponse(rawText, request.BusinessArea);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Recommended actions generation failed for {BusinessArea}", request.BusinessArea);
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

        analysis.ProcessId = processId;
        analysis.ProcessName = processName;
        AiResponseValidator.Validate(analysis);

        return analysis;
    }

    internal static CustomerRiskAssessment ParseCustomerRiskResponse(string rawText, string customerId, string companyName)
    {
        var json = StripCodeFences(rawText);

        var result = JsonSerializer.Deserialize<CustomerRiskAssessment>(json, JsonOptions);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize AI response into CustomerRiskAssessment.");

        result.CustomerId = customerId;
        result.CompanyName = companyName;
        AiResponseValidator.Validate(result);

        return result;
    }

    internal static ActivitySummaryReport ParseActivitySummaryResponse(string rawText, string department, string period)
    {
        var json = StripCodeFences(rawText);

        var result = JsonSerializer.Deserialize<ActivitySummaryReport>(json, JsonOptions);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize AI response into ActivitySummaryReport.");

        result.Department = department;
        result.Period = period;
        AiResponseValidator.Validate(result);

        return result;
    }

    internal static OpportunityAnalysisResult ParseOpportunityAnalysisResponse(string rawText, string opportunityId, string accountName)
    {
        var json = StripCodeFences(rawText);

        var result = JsonSerializer.Deserialize<OpportunityAnalysisResult>(json, JsonOptions);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize AI response into OpportunityAnalysisResult.");

        result.OpportunityId = opportunityId;
        result.AccountName = accountName;
        AiResponseValidator.Validate(result);

        return result;
    }

    internal static RecommendedActionsReport ParseRecommendedActionsResponse(string rawText, string businessArea)
    {
        var json = StripCodeFences(rawText);

        var result = JsonSerializer.Deserialize<RecommendedActionsReport>(json, JsonOptions);

        if (result is null)
            throw new InvalidOperationException("Failed to deserialize AI response into RecommendedActionsReport.");

        result.BusinessArea = businessArea;
        AiResponseValidator.Validate(result);

        return result;
    }

    private static string StripCodeFences(string rawText)
    {
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

        return json;
    }
}
