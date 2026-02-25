using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard;

public interface IWizardDiagnostics
{
    void StepChanged(string stepName);
    void TransitionBlocked(string stepName, string reason);
    void ValidationExecuted(string stepName, bool isValid);
}

public sealed class WizardTransitionState
{
    public string? BlockReason { get; }
    public bool CanProceed { get; }

    public WizardTransitionState(bool canProceed, string? blockReason = null)
    {
        CanProceed = canProceed;
        BlockReason = blockReason;
    }
}

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

public sealed class WizardDebugSnapshot
{
    public string CurrentStep { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public object? Model { get; set; }
    public IEnumerable<string>? ValidationErrors { get; set; }
}

public interface IValidator
{
    ValidationResult Validate(object model);
}

public class WizardEngine
{
    public event Action<WizardEvent>? OnWizardEvent;
    private readonly IWizardDiagnostics? _diagnostics;
    private readonly IList<WizardStepState> _steps;
    private readonly IValidator _validator;
    private int _currentIndex;
    public EditContext CurrentEditContext => CurrentStep.EditContext;
    public int CurrentIndex => _currentIndex;
    public WizardStepState CurrentStep => _steps[_currentIndex];
    public bool IsFirstStep => _currentIndex == 0;
    public bool IsLastStep => _currentIndex == _steps.Count - 1;
    public IReadOnlyList<WizardStepState> Steps => (IReadOnlyList<WizardStepState>)_steps;

    private readonly IWizardContext _context;

    public WizardEngine(IList<WizardStepState> steps, IValidator validator, IWizardContext context, IWizardDiagnostics? diagnostics = null)
    {
        _steps = steps;
        _validator = validator;
        _context = context;
        _diagnostics = diagnostics;
        _currentIndex = 0;
        SyncCurrentStepModel();
    }

    public WizardDebugSnapshot CreateSnapshot()
    {
        return new WizardDebugSnapshot
        {
            CurrentStep = CurrentStep.Name,
            IsValid = CurrentStep.IsValid,
            Model = CurrentStep.Model,
            ValidationErrors = CurrentStep.ValidationResult.Errors
        };
    }

    public void MoveBack()
    {
        if (!IsFirstStep)
        {
            _currentIndex--;
            SyncCurrentStepModel();
        }
    }

    public void MoveNext()
    {
        if (!IsLastStep)
        {
            _currentIndex++;
            SyncCurrentStepModel();
        }
    }

    public WizardTransitionState TryProceed()
    {
        var step = CurrentStep;
        step.Validate(_validator);
        _diagnostics?.ValidationExecuted(step.Name, step.IsValid);
        SyncCurrentStepModel();
        if (!step.IsValid)
        {
            _diagnostics?.TransitionBlocked(step.Name, "Validation failed.");
            OnWizardEvent?.Invoke(new WizardEvent("TransitionBlocked", step.Name));
            return new WizardTransitionState(false, "Validation failed.");
        }

        MoveNext();
        _diagnostics?.StepChanged(step.Name);
        OnWizardEvent?.Invoke(new WizardEvent("StepChanged", step.Name));
        return new WizardTransitionState(true);
    }
    private void SyncCurrentStepModel()
    {
        var model = CurrentStep.Model;
        if (model != null)
        {
            // Type-safe sync: caller must know the model type
            // If step registration tracks model types, cast and call Set<T>
            // For now, use Set<object>(model) as a fallback
            _context.Set(model);
        }
    }
}

public class WizardEvent
{
    public string EventType { get; }
    public string StepName { get; }

    public WizardEvent(string eventType, string stepName)
    {
        EventType = eventType;
        StepName = stepName;
    }
}