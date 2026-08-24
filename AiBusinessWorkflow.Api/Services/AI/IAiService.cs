using AiBusinessWorkflow.Api.Models;

namespace AiBusinessWorkflow.Api.Services.AI;

public interface IAiService
{
    Task<string> AnalyzeBusinessProcessAsync(BusinessProcess process);
    Task<string> TestAiAsync();
}
