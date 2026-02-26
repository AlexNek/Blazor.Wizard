using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard;

public sealed class WizardStepState
{
    public EditContext EditContext { get; }

    public bool IsValid => ValidationResult.IsValid;

    public object Model { get; }

    public string Name { get; }

    public ValidationResult ValidationResult { get; private set; }

    public WizardStepState(object model, string name)
    {
        Model = model;
        EditContext = new EditContext(model);
        ValidationResult = ValidationResult.Valid();
        Name = name;
    }

    public void Validate(IValidator validator)
    {
        ValidationResult = validator.Validate(Model);
    }
}
