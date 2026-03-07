using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;
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

        var strategy = InvestigationStrategyFactory.Create(plan.Strategy);
        _isVisible = strategy.IsWitnessStepVisible();
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid || !data.TryGet<InvestigationPlanStepModel>(out var plan) || plan == null)
        {
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        var strategy = InvestigationStrategyFactory.Create(plan.Strategy);

        return new StepResult
        {
            CanContinue = true,
            StayOnStep = false,
            NextStepId = strategy.GetNextStepAfterWitness()
        };
    }
}
