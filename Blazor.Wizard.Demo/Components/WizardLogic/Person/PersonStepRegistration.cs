using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed record PersonStepRegistration(
    EPersonStepId Id,
    Type StepIdType,
    Type ComponentType,
    Func<PersonStepFactoryContext, IWizardStep> StepFactory);
