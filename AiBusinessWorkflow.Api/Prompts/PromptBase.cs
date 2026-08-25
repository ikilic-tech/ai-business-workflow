namespace AiBusinessWorkflow.Api.Prompts;

/// <summary>
/// Base class for versioned prompt templates.
/// Each prompt class defines its version, purpose, and the template used to generate AI requests.
/// </summary>
public abstract class PromptBase
{
    /// <summary>Semantic version of the prompt template (e.g., "1.0.0").</summary>
    public abstract string Version { get; }

    /// <summary>Short description of what this prompt does.</summary>
    public abstract string Purpose { get; }

    /// <summary>Description of expected input data.</summary>
    public abstract string ExpectedInput { get; }

    /// <summary>Description of expected output schema.</summary>
    public abstract string ExpectedOutput { get; }
}
