namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed record PersonStepRegistration(
    EPersonStepId Id,
    Type StepIdType,
    Func<IWizardStep> StepFactory,
    Type ComponentType);
