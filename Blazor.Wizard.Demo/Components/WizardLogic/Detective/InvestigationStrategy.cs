namespace Blazor.Wizard.Demo.Components.WizardLogic.Detective;

public static class InvestigationStrategy
{
    public const string WitnessOnly = "Witness only";

    public const string ForensicsOnly = "Forensics only";

    public const string FullSweep = "Full sweep";

    public static bool IncludesWitness(string strategy)
    {
        return strategy == WitnessOnly || strategy == FullSweep;
    }

    public static bool IncludesForensics(string strategy)
    {
        return strategy == ForensicsOnly || strategy == FullSweep;
    }
}
