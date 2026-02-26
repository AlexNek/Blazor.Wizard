namespace Blazor.Wizard;

public interface IValidator
{
    ValidationResult Validate(object model);
}