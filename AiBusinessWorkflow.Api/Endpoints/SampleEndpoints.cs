using AiBusinessWorkflow.Api.Data;

namespace AiBusinessWorkflow.Api.Endpoints;

public static class SampleEndpoints
{
    public static IEndpointRouteBuilder MapSampleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/samples");

        group.MapGet("/", () => SampleDataGenerator.GetAll());

        group.MapGet("/{index:int}", (int index) =>
        {
            var sample = SampleDataGenerator.GetByIndex(index);
            return sample is not null ? Results.Ok(sample) : Results.NotFound();
        });

        group.MapGet("/customers", () => BusinessIntelligenceSampleData.GetAllCustomers());

        group.MapGet("/customers/{index:int}", (int index) =>
        {
            var sample = BusinessIntelligenceSampleData.GetCustomerByIndex(index);
            return sample is not null ? Results.Ok(sample) : Results.NotFound();
        });

        group.MapGet("/opportunities", () => BusinessIntelligenceSampleData.GetAllOpportunities());

        group.MapGet("/opportunities/{index:int}", (int index) =>
        {
            var sample = BusinessIntelligenceSampleData.GetOpportunityByIndex(index);
            return sample is not null ? Results.Ok(sample) : Results.NotFound();
        });

        group.MapGet("/activities", () => BusinessIntelligenceSampleData.GetActivitySummary());

        group.MapGet("/actions-context", () => BusinessIntelligenceSampleData.GetActionsContext());

        return app;
    }
}
