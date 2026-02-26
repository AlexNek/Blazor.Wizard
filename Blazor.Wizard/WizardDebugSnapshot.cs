namespace Blazor.Wizard;

public sealed class WizardDebugSnapshot
{
    public string CurrentStep { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public object? Model { get; set; }

    public IEnumerable<string>? ValidationErrors { get; set; }
}
