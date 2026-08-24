using AiBusinessWorkflow.Api.Services.AI;
using FluentAssertions;

namespace AiBusinessWorkflow.Tests.Unit.Services;

public class InputSanitizerTests
{
    [Fact]
    public void Sanitize_WithNormalText_ShouldReturnUnchanged()
    {
        var input = "Normal business process description";
        var result = InputSanitizer.Sanitize(input);
        result.Should().Be(input);
    }

    [Fact]
    public void Sanitize_WithDoubleCurlyBraces_ShouldBreakThem()
    {
        var input = "Attack: {{ignore instructions}}";
        var result = InputSanitizer.Sanitize(input);
        result.Should().Be("Attack: { {ignore instructions} }");
    }

    [Fact]
    public void Sanitize_WithCodeFences_ShouldBreakThem()
    {
        var input = "```json\n{\"malicious\": true}\n```";
        var result = InputSanitizer.Sanitize(input);
        result.Should().NotContain("```");
        result.Should().Contain("` ` `");
    }

    [Fact]
    public void Sanitize_WithNull_ShouldReturnNull()
    {
        var result = InputSanitizer.Sanitize(null!);
        result.Should().BeNull();
    }

    [Fact]
    public void Sanitize_WithEmpty_ShouldReturnEmpty()
    {
        var result = InputSanitizer.Sanitize(string.Empty);
        result.Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_WithNestedBraces_ShouldSanitizeAll()
    {
        var input = "{{{{nested}}}}";
        var result = InputSanitizer.Sanitize(input);
        result.Should().NotContain("{{");
        result.Should().NotContain("}}");
    }

    [Fact]
    public void Sanitize_WithSingleBraces_ShouldNotChange()
    {
        var input = "JSON: {\"key\": \"value\"}";
        var result = InputSanitizer.Sanitize(input);
        result.Should().Be(input);
    }

    [Fact]
    public void Sanitize_WithMixedContent_ShouldOnlySanitizeDangerous()
    {
        var input = "Normal text with {braces} and {{dangerous}} and ```fences```";
        var result = InputSanitizer.Sanitize(input);
        result.Should().Contain("{braces}");
        result.Should().NotContain("{{");
        result.Should().NotContain("```");
    }
}
