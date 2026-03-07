using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Models.Detective;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public sealed class WitnessInterviewStepLogic : BaseStepLogic<WitnessInterviewStepModel>
{
    private bool _isVisible = true;

    public override Type Id => typeof(WitnessInterviewStepLogic);

    public override bool IsVisible => _isVisible;

    public override async ValueTask EnterAsync(IWizardData data)
    {
        await base.EnterAsync(data);
        if (!data.TryGet<InvestigationPlanStepModel>(out var plan) || plan == null)
        {
            _isVisible = false;
            return;
        }

        _isVisible = InvestigationStrategy.IncludesWitness(plan.Strategy);
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        if (!data.TryGet<InvestigationPlanStepModel>(out var plan) || plan == null)
        {
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        var nextStep = InvestigationStrategy.IncludesForensics(plan.Strategy)
            ? typeof(ForensicsEvidenceStepLogic)
            : typeof(DetectiveAccusationStepModel);

        return new StepResult
        {
            CanContinue = true,
            StayOnStep = false,
            NextStepId = nextStep
        };
    }
}
