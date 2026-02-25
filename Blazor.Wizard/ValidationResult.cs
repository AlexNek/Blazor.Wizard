namespace Blazor.Wizard;

public sealed class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
    public static ValidationResult Valid() => new ValidationResult { IsValid = true };
}
