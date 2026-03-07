namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public static class InvestigationStrategy
{
    public const string WitnessOnly = "Witness";

    public const string ForensicsOnly = "Lab";

    public const string FullSweep = "Both";

    public static bool IncludesWitness(string strategy)
    {
        return strategy == WitnessOnly || strategy == FullSweep;
    }

    public static bool IncludesForensics(string strategy)
    {
        return strategy == ForensicsOnly || strategy == FullSweep;
    }
}
