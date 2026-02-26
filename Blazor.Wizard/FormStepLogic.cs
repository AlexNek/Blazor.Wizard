namespace Blazor.Wizard;

/// <summary>
/// Reusable form step that validates via EditContext/DataAnnotations and proceeds when valid.
/// </summary>
public sealed class FormStepLogic<TModel> : BaseStepLogic<TModel>
{
    private readonly Type _id;

    public FormStepLogic(Type id)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public override Type Id => _id;

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        return new StepResult { CanContinue = true };
    }
}