using Blazor.Wizard.Demo.Models.Detective;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public class WitnessOnlyStrategy : IInvestigationStrategy
{
    public bool IsWitnessStepVisible() => true;
    public bool IsForensicsStepVisible() => false;
    public Type GetNextStepAfterPlan() => typeof(WitnessInterviewStepLogic);
    public Type GetNextStepAfterWitness() => typeof(DetectiveAccusationStepModel);
}