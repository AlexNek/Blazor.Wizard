namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveCaseVerdict
{
    public bool IsCorrect { get; set; }

    public string Strategy { get; set; } = string.Empty;

    public string WitnessSummary { get; set; } = "Not asked";

    public string LabSummary { get; set; } = "Not asked";

    public string Suspect { get; set; } = string.Empty;

    public string MurderMethod { get; set; } = string.Empty;

    public string Motive { get; set; } = string.Empty;

    public int ConfidenceScore { get; set; }

    public string VerdictMessage { get; set; } = string.Empty;
}
