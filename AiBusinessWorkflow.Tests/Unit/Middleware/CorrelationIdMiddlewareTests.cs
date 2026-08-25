using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using AiBusinessWorkflow.Api.Middleware;

namespace AiBusinessWorkflow.Tests.Unit.Middleware;

public class CorrelationIdMiddlewareTests
{
    private static CorrelationIdMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new CorrelationIdMiddleware(next, NullLogger<CorrelationIdMiddleware>.Instance);
    }

    [Fact]
    public async Task ValidClientGuid_ShouldUseClientProvidedId()
    {
        var clientId = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = clientId;

        var middleware = CreateMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context);

        context.Response.Headers["X-Correlation-Id"].ToString().Should().Be(clientId);
        context.Items["CorrelationId"].Should().Be(clientId);
    }

    [Fact]
    public async Task InvalidClientGuid_ShouldGenerateNewId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = "not-a-guid";

        var middleware = CreateMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context);

        var responseId = context.Response.Headers["X-Correlation-Id"].ToString();
        Guid.TryParse(responseId, out _).Should().BeTrue();
        responseId.Should().NotBe("not-a-guid");
    }

    [Fact]
    public async Task NoClientHeader_ShouldGenerateNewGuid()
    {
        var context = new DefaultHttpContext();

        var middleware = CreateMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context);

        var responseId = context.Response.Headers["X-Correlation-Id"].ToString();
        responseId.Should().NotBeNullOrEmpty();
        Guid.TryParse(responseId, out _).Should().BeTrue();
    }

    [Fact]
    public async Task CorrelationId_ShouldBeStoredInHttpContextItems()
    {
        var context = new DefaultHttpContext();

        var middleware = CreateMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context);

        context.Items.Should().ContainKey("CorrelationId");
        Guid.TryParse(context.Items["CorrelationId"]!.ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public async Task Middleware_ShouldCallNext()
    {
        var called = false;
        var context = new DefaultHttpContext();

        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; });
        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }
}
