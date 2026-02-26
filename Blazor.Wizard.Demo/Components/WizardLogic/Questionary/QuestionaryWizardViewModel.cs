using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryWizardViewModel : ComponentWizardViewModel<QuestionaryModel>
{
    public QuestionaryWizardViewModel(
        IWizardResultBuilder<QuestionaryModel> resultBuilder,
        IWizardDiagnostics? diagnostics = null)
        : base(resultBuilder, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return QuestionaryStepRegistry.GetByStepIdType(step.Id).ComponentType;
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return QuestionaryStepRegistry.CreateStepFactories();
    }
}
