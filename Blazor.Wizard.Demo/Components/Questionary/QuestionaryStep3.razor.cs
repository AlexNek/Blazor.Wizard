using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryStep3 : IQuestionaryStep
{
    public QuestionaryStepId StepId => QuestionaryStepId.Step3;
    public string DisplayName => "Step 3: Favorite Color";
    // Remove Model/EditContext here to avoid conflict with .razor partial
    public QuestionaryStep3()
    {
        Model = new QuestionaryStep3Model();
        EditContext = new EditContext(Model);
    }
    public Task<StepResultNew> ExecuteAsync(StepContext context)
    {
        var isValid = EditContext.Validate();
        return Task.FromResult(new StepResultNew(isValid));
    }
}
