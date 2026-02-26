using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public sealed class QuestionaryStep2Logic : BaseStepLogic<QuestionaryStep2Model>
{
    public override Type Id => typeof(QuestionaryStep2Logic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        return new StepResult { CanContinue = true };
    }
}
