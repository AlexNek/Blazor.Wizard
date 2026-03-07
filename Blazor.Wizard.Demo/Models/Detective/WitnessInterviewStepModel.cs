using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class WitnessInterviewStepModel
{
    [Required(ErrorMessage = "Choose the witness statement you trust.")]
    public string StatementChoice { get; set; } = string.Empty;
}
