using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public sealed class QuestionaryStep1Logic : BaseStepLogic<QuestionaryStep1Model>
{
    public override Type Id => typeof(QuestionaryStep1Logic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        return new StepResult { CanContinue = true };
    }
}
