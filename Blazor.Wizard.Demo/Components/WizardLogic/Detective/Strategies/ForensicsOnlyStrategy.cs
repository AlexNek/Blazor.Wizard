using Blazor.Wizard.Demo.Models.Detective;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public class ForensicsOnlyStrategy : IInvestigationStrategy
{
    public bool IsWitnessStepVisible() => false;
    public bool IsForensicsStepVisible() => true;
    public Type GetNextStepAfterPlan() => typeof(ForensicsEvidenceStepLogic);
    public Type GetNextStepAfterWitness() => typeof(DetectiveAccusationStepModel);
}