using Blazor.Wizard.Demo.Services.Animation;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed record PersonStepFactoryContext(
    IWizardAnimationService AnimationService);
