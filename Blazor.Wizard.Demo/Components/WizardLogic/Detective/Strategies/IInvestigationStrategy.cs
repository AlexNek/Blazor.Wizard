namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public interface IInvestigationStrategy
{
    Type GetNextStepAfterPlan();

    Type GetNextStepAfterWitness();

    bool IsForensicsStepVisible();

    bool IsWitnessStepVisible();
}
