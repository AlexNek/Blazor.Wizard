using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Models.Detective;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public sealed class ForensicsEvidenceStepLogic : BaseStepLogic<ForensicsEvidenceStepModel>
{
    private bool _isVisible = true;

    public override Type Id => typeof(ForensicsEvidenceStepLogic);

    public override bool IsVisible => _isVisible;

    public override async ValueTask EnterAsync(IWizardData data)
    {
        await base.EnterAsync(data);
        if (!data.TryGet<InvestigationPlanStepModel>(out var plan) || plan == null)
        {
            _isVisible = false;
            return;
        }

        _isVisible = InvestigationStrategy.IncludesForensics(plan.Strategy);
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        return new StepResult
        {
            CanContinue = true,
            StayOnStep = false,
            NextStepId = typeof(DetectiveAccusationStepModel)
        };
    }
}
