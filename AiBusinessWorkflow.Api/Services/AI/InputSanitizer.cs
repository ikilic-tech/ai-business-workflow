namespace AiBusinessWorkflow.Api.Services.AI;

internal static class InputSanitizer
{
    internal static string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = input;

        // Break double braces (template injection)
        while (result.Contains("{{"))
            result = result.Replace("{{", "{ {");

        while (result.Contains("}}"))
            result = result.Replace("}}", "} }");

        // Break code fences (markdown injection)
        while (result.Contains("```"))
            result = result.Replace("```", "` ` `");

        // Break XML boundary tags (user_data escape)
        result = result.Replace("</user_data>", "< /user_data>", StringComparison.OrdinalIgnoreCase);
        result = result.Replace("<user_data>", "< user_data>", StringComparison.OrdinalIgnoreCase);

        return result;
    }
}
