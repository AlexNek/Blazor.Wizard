using Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

namespace Blazor.Wizard.Demo.Components.Questionary;

public partial class QuestionaryReportStep : IQuestionaryStep
{
    public string DisplayName => "Summary Report";

    public EQuestionaryStepId StepId => EQuestionaryStepId.Report;

    // Remove Model here to avoid conflict with .razor partial
    public QuestionaryReportStep()
    {
    }

    public Task<StepResultNew> ExecuteAsync(StepContext context)
    {
        // No validation, just display summary
        return Task.FromResult(new StepResultNew(true));
    }
}
