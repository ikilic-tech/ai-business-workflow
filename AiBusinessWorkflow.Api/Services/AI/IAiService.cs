using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

public interface IAiService
{
    Task<BusinessProcessAnalysis> AnalyzeBusinessProcessAsync(BusinessProcess process);
    Task<string> TestAiAsync();
}
