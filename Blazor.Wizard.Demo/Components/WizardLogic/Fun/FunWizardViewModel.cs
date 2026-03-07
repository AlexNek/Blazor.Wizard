using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.InlineFun;
using Blazor.Wizard.Demo.Models.Fun;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Fun;

public class FunWizardViewModel : ComponentWizardViewModel<FunWizardResult>
{
    public FunWizardViewModel(
        IWizardModelBuilder<FunWizardResult> mapper,
        IWizardDiagnostics? diagnostics = null)
        : base(mapper, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return step.Id switch
        {
            var id when id == typeof(FunMoodStepModel) => typeof(FunMoodStep),
            var id when id == typeof(FunSnackStepModel) => typeof(FunSnackStep),
            var id when id == typeof(FunWizardResult) => typeof(FunSummaryStep),
            _ => throw new InvalidOperationException($"Step type '{step.Id.Name}' is not registered in FunWizardViewModel.")
        };
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return
        [
            () => new FormStepLogic<FunMoodStepModel>(typeof(FunMoodStepModel)),
            () => new FormStepLogic<FunSnackStepModel>(typeof(FunSnackStepModel)),
            () => new ResultStepLogic<FunWizardResult>(typeof(FunWizardResult), data => ModelBuilder.Build(data))
        ];
    }
}
