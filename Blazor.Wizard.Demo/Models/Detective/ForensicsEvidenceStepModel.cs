using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Detective;

public class ForensicsEvidenceStepModel : IValidatableObject
{
    public string TeaQuestion { get; set; } = string.Empty;

    public string WindowQuestion { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(TeaQuestion))
        {
            yield return new ValidationResult("Choose a tea test question.", new[] { nameof(TeaQuestion) });
        }

        if (string.IsNullOrWhiteSpace(WindowQuestion))
        {
            yield return new ValidationResult("Choose a window test question.", new[] { nameof(WindowQuestion) });
        }
    }
}
