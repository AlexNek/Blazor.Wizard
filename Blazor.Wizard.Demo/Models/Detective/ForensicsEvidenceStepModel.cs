using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class ForensicsEvidenceStepModel
{
    [Required(ErrorMessage = "Choose the forensic clue that matters most.")]
    public string EvidenceChoice { get; set; } = string.Empty;
}
