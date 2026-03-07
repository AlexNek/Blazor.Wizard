using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class DetectiveCaseIntroStepModel
{
    [Range(typeof(bool), "true", "true", ErrorMessage = "Confirm you are ready to investigate the case.")]
    public bool ReadyToInvestigate { get; set; }
}
