using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Fun;

public class FunMoodStepModel
{
    [Required(ErrorMessage = "Choose a wizard mood.")]
    public string Mood { get; set; } = string.Empty;
}
