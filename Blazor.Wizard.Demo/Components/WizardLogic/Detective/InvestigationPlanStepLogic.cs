using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Models.Detective;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public sealed class InvestigationPlanStepLogic : BaseStepLogic<InvestigationPlanStepModel>
{
    public override Type Id => typeof(InvestigationPlanStepLogic);

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

        var nextStepId = InvestigationStrategy.IncludesWitness(plan.Strategy)
            ? typeof(WitnessInterviewStepLogic)
            : typeof(ForensicsEvidenceStepLogic);

        return new StepResult
        {
            CanContinue = true,
            StayOnStep = false,
            NextStepId = nextStepId
        };
    }
}
