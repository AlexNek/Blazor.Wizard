using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.DemoDevEx.Components.Wizard;

public class SummaryStepLogic : IWizardStep
{
    public Type Id => typeof(SummaryStepLogic);
    public bool IsVisible => true;

    public ValueTask EnterAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { NextStepId = null, StayOnStep = false, CanContinue = true };
    }

    public ValueTask BeforeLeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when the wizard leaves this step. Allows cleanup or saving state.
    /// </summary>
    /// <param name="data">The wizard data context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public ValueTask LeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ValidateAsync(IWizardData data)
    {
        return ValueTask.FromResult(true);
    }
}