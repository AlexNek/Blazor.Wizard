using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryWizardViewModel : ComponentWizardViewModel<QuestionaryModel>
{
    public QuestionaryWizardViewModel(IWizardDiagnostics? diagnostics = null)
        : base(new QuestionaryResultBuilder(), diagnostics)
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
