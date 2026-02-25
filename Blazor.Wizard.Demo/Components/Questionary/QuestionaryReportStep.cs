using System.Threading.Tasks;
using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;
using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryReportStep : IQuestionaryStep
{
    public QuestionaryStepId StepId => QuestionaryStepId.Report;
    public string DisplayName => "Summary Report";
    // Remove Model here to avoid conflict with .razor partial
    public QuestionaryReportStep() { }
    public Task<StepResultNew> ExecuteAsync(StepContext context)
    {
        // No validation, just display summary
        return Task.FromResult(new StepResultNew(true));
    }
}
