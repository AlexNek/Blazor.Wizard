namespace Blazor.Wizard;

/// <summary>
///     Represents the result of a wizard step evaluation, controlling navigation and flow.
/// </summary>
public sealed class StepResult
{
    /// <summary>
    ///     Indicates whether the wizard is allowed to proceed to the next step.
    ///     If false, the wizard cannot advance.
    /// </summary>
    public bool CanContinue { get; init; }

    /// <summary>
    ///     Specifies the type of the next step to navigate to, if applicable.
    ///     Null means default navigation.
    /// </summary>
    public Type? NextStepId { get; init; }

    /// <summary>
    ///     Indicates whether the wizard should remain on the current step, even if CanContinue is true.
    ///     Useful for cases where logic allows continuation but UI or validation requires staying.
    /// </summary>
    public bool StayOnStep { get; init; }
}