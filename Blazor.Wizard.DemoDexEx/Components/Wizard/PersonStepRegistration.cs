using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.DemoDevEx.Components.Wizard;

public sealed record PersonStepRegistration(
    EPersonStepId Id,
    Type StepIdType,
    Func<IWizardStep> StepFactory,
    Type ComponentType);
