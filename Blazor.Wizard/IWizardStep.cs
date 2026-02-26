namespace Blazor.Wizard;

/// <summary>
/// Represents a step in a wizard flow.
/// Provides lifecycle methods, evaluation, validation, and navigation support.
/// </summary>
public interface IWizardStep
{
    /// <summary>
    /// Gets the unique identifier (type) for this step.
    /// </summary>
    Type Id { get; }

    /// <summary>
    /// Gets a value indicating whether this step is visible in the wizard navigation.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Called when the wizard enters this step. Allows initialization from wizard data.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask EnterAsync(IWizardData data);

    /// <summary>
    /// Evaluates the step's business logic and validation.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <param name="validation">The validation result to update.</param>
    /// <returns>A <see cref="StepResult"/> describing navigation and validation outcome.</returns>
    StepResult Evaluate(IWizardData data, ValidationResult validation);

    /// <summary>
    /// Called before leaving this step. Allows pre-leave logic or validation.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask BeforeLeaveAsync(IWizardData data);

    /// <summary>
    /// Called when the wizard leaves this step. Allows cleanup or saving state.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask LeaveAsync(IWizardData data);

    /// <summary>
    /// Validates the step's model and state.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task returning true if valid, false otherwise.</returns>
    ValueTask<bool> ValidateAsync(IWizardData data);

    /// <summary>
    /// Gets additional parameters for the step's UI component.
    /// </summary>
    /// <returns>A dictionary of parameter names and values for DynamicComponent.</returns>
    Dictionary<string, object> GetComponentParameters() => new();
}
