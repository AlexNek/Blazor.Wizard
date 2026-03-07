using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class ForensicsEvidenceStepModel
{
    [Required(ErrorMessage = "Choose one clue.")]
    public string EvidenceChoice { get; set; } = string.Empty;
}
