using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Integration;

public class RateLimitingEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitingEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task IntelligenceEndpoint_WhenLimitIsExceeded_Returns429ProblemDetailsAndRetryAfter()
    {
        var customer = new
        {
            companyName = "Test Corp",
            industry = "Technology",
            employeeCount = 100,
            annualRevenue = 5000000,
            contactName = "John Doe",
            contactEmail = "john@test.com",
            accountAge = "2 years",
            paymentHistory = "Always on time payments",
            activities = new[]
            {
                new
                {
                    type = "Meeting",
                    date = "2024-01-15",
                    description = "Quarterly review meeting",
                    outcome = "Positive feedback"
                }
            }
        };

        for (var requestNumber = 0; requestNumber < 20; requestNumber++)
        {
            var successfulResponse = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", customer);
            successfulResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var rejectedResponse = await _client.PostAsJsonAsync("/api/intelligence/customer-risk", customer);

        rejectedResponse.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        rejectedResponse.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        rejectedResponse.Headers.RetryAfter.Should().NotBeNull();
        rejectedResponse.Headers.RetryAfter!.Delta.Should().NotBeNull();
        rejectedResponse.Headers.RetryAfter.Delta!.Value.TotalSeconds.Should().BePositive();

        var problem = await rejectedResponse.Content.ReadFromJsonAsync<RateLimitProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Too Many Requests");
        problem.Detail.Should().Be("Rate limit exceeded. Please try again later.");
        problem.Status.Should().Be((int)HttpStatusCode.TooManyRequests);
    }

    private sealed class RateLimitProblemDetails
    {
        public string? Title { get; init; }
        public string? Detail { get; init; }
        public int? Status { get; init; }
    }
}
