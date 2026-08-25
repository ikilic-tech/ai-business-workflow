using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.FileProviders;
using AiBusinessWorkflow.Api.Middleware;

namespace AiBusinessWorkflow.Tests.Unit.Middleware;

public class ApiKeyAuthMiddlewareTests
{
    private static ApiKeyAuthMiddleware CreateMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        return new ApiKeyAuthMiddleware(
            next,
            configuration,
            NullLogger<ApiKeyAuthMiddleware>.Instance);
    }

    private static IConfiguration BuildConfig(string[]? apiKeys = null)
    {
        var dict = new Dictionary<string, string?>();
        if (apiKeys is not null)
        {
            for (var i = 0; i < apiKeys.Length; i++)
                dict[$"Authentication:ApiKeys:{i}"] = apiKeys[i];
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
    }

    private static HttpContext CreateContext(string path, string? apiKey = null, string environment = "Development")
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        if (apiKey is not null)
            context.Request.Headers["X-Api-Key"] = apiKey;

        var env = new FakeHostEnvironment(environment);
        var services = new ServiceCollection();
        services.AddSingleton<IHostEnvironment>(env);
        context.RequestServices = services.BuildServiceProvider();

        return context;
    }

    [Theory]
    [InlineData("/api/health")]
    [InlineData("/api/health/ready")]
    [InlineData("/api/samples")]
    [InlineData("/api/samples/processes")]
    [InlineData("/swagger")]
    [InlineData("/swagger/v1/swagger.json")]
    public async Task PublicPaths_ShouldBypassAuth(string path)
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["test-key"]));
        var context = CreateContext(path);

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    [Theory]
    [InlineData("/api/healthPRIVATE")]
    [InlineData("/api/samplesx")]
    [InlineData("/swaggerx")]
    public async Task NonPublicPathsWithSimilarPrefix_ShouldRequireAuth(string path)
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["test-key"]));
        var context = CreateContext(path);

        await middleware.InvokeAsync(context);

        called.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task ValidApiKey_ShouldCallNext()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["my-secret-key"]));
        var context = CreateContext("/api/intelligence/customer-risk", apiKey: "my-secret-key");

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidApiKey_ShouldReturn403()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["correct-key"]));
        var context = CreateContext("/api/intelligence/customer-risk", apiKey: "wrong-key");

        await middleware.InvokeAsync(context);

        called.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task MissingApiKey_ShouldReturn401()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["some-key"]));
        var context = CreateContext("/api/intelligence/customer-risk");

        await middleware.InvokeAsync(context);

        called.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task NoKeysConfigured_InDevelopment_ShouldAllowThrough()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig());
        var context = CreateContext("/api/intelligence/customer-risk", environment: "Development");

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    [Fact]
    public async Task NoKeysConfigured_InProduction_ShouldReturn401()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig());
        var context = CreateContext("/api/intelligence/customer-risk", environment: "Production");

        await middleware.InvokeAsync(context);

        called.Should().BeFalse();
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task MultipleConfiguredKeys_AnyValidKeyShouldWork()
    {
        var called = false;
        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; }, BuildConfig(["key-1", "key-2", "key-3"]));
        var context = CreateContext("/api/intelligence/customer-risk", apiKey: "key-2");

        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    private class FakeHostEnvironment : IHostEnvironment
    {
        public FakeHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Test";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
