namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public sealed record StepRegistration(
    EQuestionaryStepId Id,
    Type StepType,
    Func<IWizardStep> StepFactory,
    Type ComponentType);