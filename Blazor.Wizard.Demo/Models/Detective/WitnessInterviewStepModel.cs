using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class WitnessInterviewStepModel
{
    [Required(ErrorMessage = "Choose one statement.")]
    public string StatementChoice { get; set; } = string.Empty;
}
