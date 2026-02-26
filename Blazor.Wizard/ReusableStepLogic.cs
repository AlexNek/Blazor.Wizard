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

/// <summary>
/// Reusable summary/final step that builds a model for rendering and always allows completion.
/// </summary>
public sealed class ResultStepLogic<TResultModel> : IWizardStep
{
    private readonly Func<IWizardData, TResultModel> _resultBuilder;
    private readonly Type _id;
    private TResultModel? _model;

    public ResultStepLogic(Type id, Func<IWizardData, TResultModel> resultBuilder)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
    }

    public Type Id => _id;
    public bool IsVisible => true;

    public ValueTask EnterAsync(IWizardData data)
    {
        _model = _resultBuilder(data);
        return ValueTask.CompletedTask;
    }

    public StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { CanContinue = true };
    }

    public ValueTask BeforeLeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask LeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ValidateAsync(IWizardData data)
    {
        return ValueTask.FromResult(true);
    }

    public Dictionary<string, object> GetComponentParameters()
    {
        return new Dictionary<string, object>
        {
            ["Model"] = _model!
        };
    }
}
