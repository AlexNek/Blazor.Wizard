using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryStep2 : IQuestionaryStep
{
    public QuestionaryStepId StepId => QuestionaryStepId.Step2;
    public string DisplayName => "Step 2: Age";
    // Remove Model/EditContext here to avoid conflict with .razor partial
    public QuestionaryStep2()
    {
        Model = new QuestionaryStep2Model();
        EditContext = new EditContext(Model);
    }
    public Task<StepResultNew> ExecuteAsync(StepContext context)
    {
        var isValid = EditContext.Validate();
        return Task.FromResult(new StepResultNew(isValid));
    }
}
