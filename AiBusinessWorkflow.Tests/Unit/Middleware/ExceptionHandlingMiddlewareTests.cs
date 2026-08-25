using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.FileProviders;
using AiBusinessWorkflow.Api.Middleware;

namespace AiBusinessWorkflow.Tests.Unit.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static ExceptionHandlingMiddleware CreateMiddleware(
        RequestDelegate next,
        string environment = "Production")
    {
        return new ExceptionHandlingMiddleware(
            next,
            NullLogger<ExceptionHandlingMiddleware>.Instance,
            new FakeHostEnvironment(environment));
    }

    [Fact]
    public async Task NoException_ShouldCallNextNormally()
    {
        var called = false;
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = CreateMiddleware(_ => { called = true; return Task.CompletedTask; });
        await middleware.InvokeAsync(context);

        called.Should().BeTrue();
    }

    [Fact]
    public async Task UnhandledException_ShouldReturn500()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Something broke"));
        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Contain("application/json");
    }

    [Fact]
    public async Task UnhandledException_InDevelopment_ShouldIncludeExceptionDetails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Detailed error message"),
            environment: "Development");

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<JsonElement>(body);

        problem.GetProperty("detail").GetString().Should().Contain("Detailed error message");
    }

    [Fact]
    public async Task UnhandledException_InProduction_ShouldNotIncludeExceptionDetails()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = CreateMiddleware(
            _ => throw new InvalidOperationException("Secret internal error"),
            environment: "Production");

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        body.Should().NotContain("Secret internal error");
        body.Should().Contain("An internal error occurred");
    }

    [Fact]
    public async Task UnhandledException_ShouldIncludeCorrelationId_WhenPresent()
    {
        var correlationId = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Items["CorrelationId"] = correlationId;

        var middleware = CreateMiddleware(_ => throw new Exception("test"));
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();

        body.Should().Contain(correlationId);
    }

    [Fact]
    public async Task UnhandledException_ShouldReturnProblemDetailsFormat()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();

        var middleware = CreateMiddleware(_ => throw new Exception("test"));
        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problem = JsonSerializer.Deserialize<JsonElement>(body);

        problem.GetProperty("status").GetInt32().Should().Be(500);
        problem.GetProperty("title").GetString().Should().Be("An unexpected error occurred");
        problem.GetProperty("instance").GetString().Should().Be("/api/test");
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
