using System.Threading.Tasks;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public interface IQuestionaryStep
{
    EQuestionaryStepId StepId { get; }
    string DisplayName { get; }
    Task<StepResultNew> ExecuteAsync(StepContext context);
}