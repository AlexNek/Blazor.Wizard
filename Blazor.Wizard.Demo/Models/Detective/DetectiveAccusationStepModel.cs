using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveAccusationStepModel
{
    [Required(ErrorMessage = "Choose suspect.")]
    public string Suspect { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose method.")]
    public string MurderMethod { get; set; } = string.Empty;
}
