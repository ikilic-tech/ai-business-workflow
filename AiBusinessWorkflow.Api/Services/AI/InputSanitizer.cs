namespace AiBusinessWorkflow.Api.Services.AI;

internal static class InputSanitizer
{
    internal static string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = input;

        while (result.Contains("{{"))
            result = result.Replace("{{", "{ {");

        while (result.Contains("}}"))
            result = result.Replace("}}", "} }");

        while (result.Contains("```"))
            result = result.Replace("```", "` ` `");

        return result;
    }
}
