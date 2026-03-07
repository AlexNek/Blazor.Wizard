using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class InvestigationPlanStepModel
{
    [Required(ErrorMessage = "Choose one plan.")]
    public string Strategy { get; set; } = string.Empty;
}
