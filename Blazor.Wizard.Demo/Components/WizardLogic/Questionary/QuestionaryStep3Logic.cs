using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryStep3Logic : GeneralStepLogic<QuestionaryStep3Model>
    {
        public override Type Id => typeof(QuestionaryStep3Logic);
        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            // Data Annotations validation has already run and set validation.IsValid
            // Only add additional business logic validation here if needed
            // Do NOT override validation.IsValid unless you have additional rules
            return new StepResult { CanContinue = validation.IsValid };
        }
    }
}