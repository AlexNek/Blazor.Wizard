using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class WitnessInterviewStepModel : IValidatableObject
{
    public string IvyQuestion { get; set; } = string.Empty;

    public string NoraQuestion { get; set; } = string.Empty;

    public string MarcosQuestion { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(IvyQuestion))
        {
            yield return new ValidationResult("Choose a question for Ivy.", new[] { nameof(IvyQuestion) });
        }

        if (string.IsNullOrWhiteSpace(NoraQuestion))
        {
            yield return new ValidationResult("Choose a question for Nora.", new[] { nameof(NoraQuestion) });
        }

        if (string.IsNullOrWhiteSpace(MarcosQuestion))
        {
            yield return new ValidationResult("Choose a question for Marcos.", new[] { nameof(MarcosQuestion) });
        }
    }
}
