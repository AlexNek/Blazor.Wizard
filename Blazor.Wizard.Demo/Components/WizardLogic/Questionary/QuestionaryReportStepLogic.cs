using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public sealed class QuestionaryReportStepLogic : IWizardStep
{
    private readonly QuestionaryResultBuilder _resultBuilder;

    private QuestionaryModel _model = new();

    public Type Id => typeof(QuestionaryReportStepLogic);

    public bool IsVisible => true;

    public QuestionaryReportStepLogic(QuestionaryResultBuilder resultBuilder)
    {
        _resultBuilder = resultBuilder;
    }

    public ValueTask BeforeLeaveAsync(IWizardData data)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask EnterAsync(IWizardData data)
    {
        _model = _resultBuilder.Build(data);
        return ValueTask.CompletedTask;
    }

    public StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        return new StepResult { CanContinue = true };
    }

    public Dictionary<string, object> GetComponentParameters()
    {
        return new Dictionary<string, object> { ["Model"] = _model };
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
