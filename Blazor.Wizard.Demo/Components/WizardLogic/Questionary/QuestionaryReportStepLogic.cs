using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary
{
    public class QuestionaryReportStepLogic : BaseStepLogic<QuestionaryModel>
    {
        public override Type Id => typeof(QuestionaryReportStepLogic);

        // Use a factory to build the summary model
        public QuestionaryReportStepLogic() : base(() => new QuestionaryModel())
        {
        }

        public override ValueTask EnterAsync(IWizardData data)
        {
            // Build the summary model from all step data
            var model = GetModel();
            
            if (data.TryGet<QuestionaryStep1Model>(out var step1))
            {
                model.Name = step1.Name;
            }
            
            if (data.TryGet<QuestionaryStep2Model>(out var step2))
            {
                model.Age = step2.Age;
            }
            
            if (data.TryGet<QuestionaryStep3Model>(out var step3))
            {
                model.FavoriteColor = step3.FavoriteColor;
            }

            return ValueTask.CompletedTask;
        }

        public override StepResult Evaluate(IWizardData data, ValidationResult validation)
        {
            validation.IsValid = true;
            return new StepResult { CanContinue = true };
        }
    }
}