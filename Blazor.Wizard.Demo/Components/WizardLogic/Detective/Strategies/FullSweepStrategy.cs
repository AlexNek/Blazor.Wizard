namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public class FullSweepStrategy : IInvestigationStrategy
{
    public bool IsWitnessStepVisible() => true;
    public bool IsForensicsStepVisible() => true;
    public Type GetNextStepAfterPlan() => typeof(WitnessInterviewStepLogic);
    public Type GetNextStepAfterWitness() => typeof(ForensicsEvidenceStepLogic);
}