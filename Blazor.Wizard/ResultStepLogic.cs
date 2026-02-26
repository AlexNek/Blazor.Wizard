namespace Blazor.Wizard;

/// <summary>
/// Reusable summary/final step that builds a model for rendering and always allows completion.
/// </summary>
public sealed class ResultStepLogic<TResultModel> : IWizardStep
{
    private readonly Type _id;

    private readonly Func<IWizardData, TResultModel> _resultBuilder;

    private TResultModel? _model;

    public Type Id => _id;

    public bool IsVisible => true;

    public ResultStepLogic(Type id, Func<IWizardData, TResultModel> resultBuilder)
    {
        _id = id ?? throw new ArgumentNullException(nameof(id));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
    }

    public ValueTask BeforeLeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask EnterAsync(IWizardData data)
    {
        _model = _resultBuilder(data);
        return ValueTask.CompletedTask;
    }

    public StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { CanContinue = true };
    }

    public Dictionary<string, object> GetComponentParameters()
    {
        return new Dictionary<string, object> { ["Model"] = _model! };
    }

    public ValueTask LeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> ValidateAsync(IWizardData data)
    {
        return ValueTask.FromResult(true);
    }
}
