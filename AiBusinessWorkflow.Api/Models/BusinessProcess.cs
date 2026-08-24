namespace AiBusinessWorkflow.Api.Models;

public class BusinessProcess
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InputData { get; set; } = string.Empty;

    public string Goal { get; set; } = string.Empty;
}