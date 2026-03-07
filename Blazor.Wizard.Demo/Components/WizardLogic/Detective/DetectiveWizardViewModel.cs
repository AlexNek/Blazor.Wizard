using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.InlineDetective;
using Blazor.Wizard.Demo.Models.Detective;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public class DetectiveWizardViewModel : ComponentWizardViewModel<DetectiveCaseVerdict>
{
    public DetectiveWizardViewModel(
        IWizardModelBuilder<DetectiveCaseVerdict> mapper,
        IWizardDiagnostics? diagnostics = null)
        : base(mapper, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return step.Id switch
        {
            var id when id == typeof(DetectiveCaseIntroStepModel) => typeof(DetectiveCaseIntroStep),
            var id when id == typeof(InvestigationPlanStepLogic) => typeof(InvestigationPlanStep),
            var id when id == typeof(WitnessInterviewStepLogic) => typeof(WitnessInterviewStep),
            var id when id == typeof(ForensicsEvidenceStepLogic) => typeof(ForensicsEvidenceStep),
            var id when id == typeof(DetectiveAccusationStepModel) => typeof(DetectiveAccusationStep),
            var id when id == typeof(DetectiveCaseVerdict) => typeof(DetectiveVerdictStep),
            _ => throw new InvalidOperationException($"Step type '{step.Id.Name}' is not registered in DetectiveWizardViewModel.")
        };
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return
        [
            () => new FormStepLogic<DetectiveCaseIntroStepModel>(typeof(DetectiveCaseIntroStepModel)),
            () => new InvestigationPlanStepLogic(),
            () => new WitnessInterviewStepLogic(),
            () => new ForensicsEvidenceStepLogic(),
            () => new FormStepLogic<DetectiveAccusationStepModel>(typeof(DetectiveAccusationStepModel)),
            () => new ResultStepLogic<DetectiveCaseVerdict>(typeof(DetectiveCaseVerdict), data => ModelBuilder.Build(data))
        ];
    }
}
