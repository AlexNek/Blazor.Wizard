using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryResultBuilder : IWizardResultBuilder<QuestionaryModel>
{
    public QuestionaryModel Build(IWizardData data)
    {
        var result = new QuestionaryModel();

        // Get data from each step
        if (data.TryGet<QuestionaryStep1Model>(out var step1))
        {
            result.Name = step1.Name;
        }

        if (data.TryGet<QuestionaryStep2Model>(out var step2))
        {
            result.Age = step2.Age;
        }

        if (data.TryGet<QuestionaryStep3Model>(out var step3))
        {
            result.FavoriteColor = step3.FavoriteColor;
        }

        return result;
    }
}