using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryReportStepLogic : BaseStepLogic<QuestionaryModel>
    {
        public override Type Id => typeof(QuestionaryReportStepLogic);
        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            validation.IsValid = true;
            return new StepResult { CanContinue = true };
        }
    }
}