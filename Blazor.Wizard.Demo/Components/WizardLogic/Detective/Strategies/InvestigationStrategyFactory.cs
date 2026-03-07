namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective.Strategies;

public static class InvestigationStrategyFactory
{
    public const string WitnessOnly = "Witness";
    public const string ForensicsOnly = "Lab";
    public const string FullSweep = "Both";

    public static IInvestigationStrategy Create(string strategyName)
    {
        return strategyName switch
            {
                WitnessOnly => new WitnessOnlyStrategy(),
                ForensicsOnly => new ForensicsOnlyStrategy(),
                FullSweep => new FullSweepStrategy(),
                _ => throw new ArgumentException($"Unknown strategy: {strategyName}", nameof(strategyName))
            };
    }
}