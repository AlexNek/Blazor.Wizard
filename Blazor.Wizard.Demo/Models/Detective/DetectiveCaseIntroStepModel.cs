using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveCaseIntroStepModel
{
    [Range(typeof(bool), "true", "true", ErrorMessage = "Please check this box.")]
    public bool ReadyToInvestigate { get; set; }
}
