namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public class FullSweepStrategy : IInvestigationStrategy
{
    public Type GetNextStepAfterPlan() => typeof(WitnessInterviewStepLogic);

    public Type GetNextStepAfterWitness() => typeof(ForensicsEvidenceStepLogic);

    public bool IsForensicsStepVisible() => true;

    public bool IsWitnessStepVisible() => true;
}
