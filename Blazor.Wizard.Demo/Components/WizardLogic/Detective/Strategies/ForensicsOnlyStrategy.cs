using Blazor.Wizard.Demo.Models.Detective;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public class ForensicsOnlyStrategy : IInvestigationStrategy
{
    public Type GetNextStepAfterPlan() => typeof(ForensicsEvidenceStepLogic);

    public Type GetNextStepAfterWitness() => typeof(DetectiveAccusationStepModel);

    public bool IsForensicsStepVisible() => true;

    public bool IsWitnessStepVisible() => false;
}
