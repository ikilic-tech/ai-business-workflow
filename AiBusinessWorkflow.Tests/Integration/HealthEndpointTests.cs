using System.Net;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Integration;

public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnJsonWithStatus()
    {
        var response = await _client.GetAsync("/api/health");
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("status");
        content.Should().Contain("entries");
    }
}
