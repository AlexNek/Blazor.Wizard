using System.ComponentModel.DataAnnotations;

namespace Blazor.Wizard.Demo.Models.Fun;

public class FunSnackStepModel : IValidatableObject
{
    public bool Tacos { get; set; }

    public bool Donuts { get; set; }

    public bool Popcorn { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Tacos && !Donuts && !Popcorn)
        {
            yield return new ValidationResult(
                "Pick at least one snack for the quest.",
                new[] { nameof(Tacos) });
        }
    }
}
