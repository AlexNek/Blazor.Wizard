using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed record PersonStepDefinition(
    EPersonStepId Id,
    Type StepIdType,
    Type ComponentType,
    Func<IServiceProvider, IWizardStep> StepFactory);
