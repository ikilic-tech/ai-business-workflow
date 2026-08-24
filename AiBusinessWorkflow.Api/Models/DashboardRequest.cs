namespace AiBusinessWorkflow.Api.Models;

public class DashboardRequest
{
    public CustomerProfile? Customer { get; set; }

    public Opportunity? Opportunity { get; set; }

    public ActivitySummaryRequest? Activities { get; set; }

    public RecommendedActionsRequest? ActionsContext { get; set; }
}
