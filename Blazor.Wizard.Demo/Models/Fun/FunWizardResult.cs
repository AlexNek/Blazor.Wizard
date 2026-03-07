namespace Blazor.Wizard.Demo.Models.Fun;

public class FunWizardResult
{
    public string Mood { get; set; } = string.Empty;

    public bool Tacos { get; set; }

    public bool Donuts { get; set; }

    public bool Popcorn { get; set; }

    public string SnacksSummary
    {
        get
        {
            var snacks = new List<string>();
            if (Tacos)
            {
                snacks.Add("Tacos");
            }

            if (Donuts)
            {
                snacks.Add("Donuts");
            }

            if (Popcorn)
            {
                snacks.Add("Popcorn");
            }

            return snacks.Count == 0 ? "None" : string.Join(", ", snacks);
        }
    }
}
