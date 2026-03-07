using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveAccusationStepModel
{
    [Required(ErrorMessage = "Pick your main suspect.")]
    public string Suspect { get; set; } = string.Empty;

    [Required(ErrorMessage = "Pick the murder method.")]
    public string MurderMethod { get; set; } = string.Empty;
}
