using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard;

public class WizardEngine
{
    public event Action<WizardEvent>? OnWizardEvent;

    private readonly IWizardContext _context;

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

    public WizardEngine(
        IList<WizardStepState> steps,
        IValidator validator,
        IWizardContext context,
        IWizardDiagnostics? diagnostics = null)
    {
        _steps = steps;
        _validator = validator;
        _context = context;
        _diagnostics = diagnostics;
        _currentIndex = 0;
        SyncCurrentStepModel();
        _diagnostics?.WizardStarted(CurrentStep.Name);
        _diagnostics?.StepEntered(CurrentStep.Name);
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
            _diagnostics?.StepEntered(CurrentStep.Name);
            SyncCurrentStepModel();
        }
    }

    public void MoveNext()
    {
        if (!IsLastStep)
        {
            _currentIndex++;
            _diagnostics?.StepEntered(CurrentStep.Name);
            SyncCurrentStepModel();
        }
    }

    public WizardTransitionState TryProceed()
    {
        var step = CurrentStep;
        var isValid = step.EditContext.Validate();
        _diagnostics?.ValidationExecuted(step.Name, isValid);

        if (!isValid)
        {
            var errors = step.EditContext.GetValidationMessages();
            var errorMsg = string.Join(", ", errors);
            _diagnostics?.TransitionBlocked(step.Name, $"Validation failed: {errorMsg}");
            OnWizardEvent?.Invoke(new WizardEvent("TransitionBlocked", step.Name));
            return new WizardTransitionState(false, "Validation failed.");
        }

        SyncCurrentStepModel();
        _diagnostics?.StepCompleted(step.Name);

        if (IsLastStep)
        {
            _diagnostics?.WizardCompleted(step.Name);
            return new WizardTransitionState(true);
        }

        MoveNext();
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
