using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryStep1
{
    [Parameter, EditorRequired]
    public required QuestionaryStep1Model Model { get; set; }
    [Parameter, EditorRequired]
    public required EditContext EditContext { get; set; }
    public QuestionaryStepId StepId => QuestionaryStepId.Step1;
    public string DisplayName => "Step 1: Name";
    // Remove Model/EditContext here to avoid conflict with .razor partial
    public QuestionaryStep1()
    {
        Model = new QuestionaryStep1Model();
        EditContext = new EditContext(Model);
    }
    public Task<StepResultNew> ExecuteAsync(StepContext context)
    {
        var isValid = EditContext.Validate();
        return Task.FromResult(new StepResultNew(isValid));
    }
}