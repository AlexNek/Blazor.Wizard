namespace Blazor.Wizard.Core;

/// <summary>
/// Represents the serializable state of a wizard.
/// </summary>
public sealed class WizardState
{
    /// <summary>
    /// Current step index in the wizard flow.
    /// </summary>
    public int CurrentStepIndex { get; set; }

    /// <summary>
    /// Serialized wizard data models, keyed by type name.
    /// </summary>
    public Dictionary<string, string> SerializedData { get; set; } = new();

    /// <summary>
    /// Timestamp when state was saved.
    /// </summary>
    public DateTime SavedAt { get; set; }
}
