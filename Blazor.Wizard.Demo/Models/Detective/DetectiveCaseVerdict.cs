namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveCaseVerdict
{
    public bool IsCorrect { get; set; }

    public string Strategy { get; set; } = string.Empty;

    public string StatementChoice { get; set; } = "Not investigated";

    public string EvidenceChoice { get; set; } = "Not investigated";

    public string Suspect { get; set; } = string.Empty;

    public string MurderMethod { get; set; } = string.Empty;

    public int ConfidenceScore { get; set; }

    public string VerdictMessage { get; set; } = string.Empty;
}
