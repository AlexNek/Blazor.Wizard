using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard;

public class WizardViewModel<TStep, TData, TResult>
    where TStep : IWizardStep
    where TData : IWizardData, new()
{
    public event Action? StateChanged;
    private readonly TData _data = new();
    private readonly IWizardResultBuilder<TResult> _resultBuilder;
    public bool CanProceed { get; protected set; }
    public TData Data => _data;
    public WizardFlow<int>? Flow { get; protected set; }
    public WizardStepFactory StepFactory { get; } = new();
    public List<TStep> Steps { get; protected set; } = new();

    public WizardViewModel(IWizardResultBuilder<TResult> resultBuilder)
    {
        _resultBuilder = resultBuilder;
    }

    public virtual async Task BackAsync()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index <= 0)
        {
            return;
        }

        UnsubscribeFromCurrentStepChanges();
        var prevIndex = Flow.Index - 1;
        while (prevIndex >= 0 && !Steps[prevIndex].IsVisible) prevIndex--;
        if (prevIndex >= 0)
        {
            Flow.Index = prevIndex;
            await Steps[Flow.Index].EnterAsync(_data);
        }

        SubscribeToCurrentStepChanges();
        await UpdateCanProceedAsync();
    }

    public virtual async Task<TResult?> FinishAsync()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
        {
            return default;
        }

        var step = Steps[Flow.Index];
        await step.BeforeLeaveAsync(_data);

        var validation = new ValidationResult { IsValid = await step.ValidateAsync(_data) };
        var stepResult = step.Evaluate(_data, validation);
        var canProceed = validation.IsValid && stepResult.CanContinue && !stepResult.StayOnStep;

        if (canProceed)
        {
            return _resultBuilder.Build(_data);
        }

        await UpdateCanProceedAsync();
        return default;
    }

    public virtual void Initialize(IEnumerable<Func<TStep>>? stepFactories)
    {
        Steps.Clear();
        if (stepFactories != null)
        {
            foreach (var factory in stepFactories)
            {
                Steps.Add(factory());
            }
        }

        Flow = new WizardFlow<int>(_data);
        foreach (var step in Steps)
        {
            Flow.Add(step);
        }
    }

    public virtual async Task<bool> NextAsync()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
        {
            return false;
        }

        var step = Steps[Flow.Index];
        await step.BeforeLeaveAsync(_data);
        var validation = new ValidationResult { IsValid = await step.ValidateAsync(_data) };
        var stepResult = step.Evaluate(_data, validation);
        var canProceed = validation.IsValid && stepResult.CanContinue && !stepResult.StayOnStep;
        if (canProceed)
        {
            await step.LeaveAsync(_data);
            // Unsubscribe from current step before moving
            UnsubscribeFromCurrentStepChanges();

            var nextStepIndex = FindNextStepIndex(stepResult.NextStepId);
            if (nextStepIndex >= 0)
            {
                Flow.Index = nextStepIndex;
                await Steps[Flow.Index].EnterAsync(_data);
            }
            else
            {
                Flow.Index++;
                if (Flow.Index < Steps.Count)
                {
                    await Steps[Flow.Index].EnterAsync(_data);
                }
            }

            // Subscribe to new step
            SubscribeToCurrentStepChanges();
            await UpdateCanProceedAsync();
            return true;
        }

        await UpdateCanProceedAsync();
        return false;
    }

    public virtual void Reset()
    {
        UnsubscribeFromCurrentStepChanges();
        Flow = null;
        Steps.Clear();
        // IMPORTANT: Do NOT re-create _data here. It must persist for validation and error messages to work across steps.
        // If you need to clear wizard data, clear its properties, not the object itself.
    }

    public virtual async Task StartAsync()
    {
        if (Flow != null)
        {
            await Flow.StartAsync();
        }

        SubscribeToCurrentStepChanges();
        await UpdateCanProceedAsync();
    }

    protected virtual int FindNextStepIndex(Type? nextStepType)
    {
        if (nextStepType == null)
        {
            return -1;
        }

        for (var i = 0; i < Steps.Count; i++)
        {
            if (Steps[i].Id == nextStepType)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     WARNING: async void is required here because EditContext.OnFieldChanged expects a void event handler.
    ///     All exceptions must be caught and logged in overridden method to avoid unhandled errors.
    /// </summary>
    protected virtual async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        await UpdateCanProceedAsync();
    }

    protected void RaiseStateChanged()
    {
        StateChanged?.Invoke();
    }

    public virtual void SubscribeToCurrentStepChanges()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
        {
            return;
        }

        var step = Steps[Flow.Index];
        // Use dynamic dispatch to handle different BaseStepLogic<T> types
        if (TryGetEditContext(step, out var editContext) && editContext != null)
        {
            editContext.OnFieldChanged += OnFieldChanged;
        }
    }

    protected virtual bool TryGetEditContext(TStep step, out EditContext? editContext)
    {
        editContext = null;

        // Use reflection to call GetEditContext() on BaseStepLogic<T> regardless of T
        var method = step.GetType().GetMethod("GetEditContext");
        if (method != null)
        {
            editContext = method.Invoke(step, null) as EditContext;
            return editContext != null;
        }

        return false;
    }

    public virtual void UnsubscribeFromCurrentStepChanges()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
        {
            return;
        }

        var step = Steps[Flow.Index];
        // Use dynamic dispatch to handle different BaseStepLogic<T> types
        if (TryGetEditContext(step, out var editContext) && editContext != null)
        {
            editContext.OnFieldChanged -= OnFieldChanged;
        }
    }

    protected virtual async Task UpdateCanProceedAsync()
    {
        if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
        {
            CanProceed = false;
            RaiseStateChanged();
            return;
        }

        var step = Steps[Flow.Index];

        // Only validate form fields (data annotations), not business logic.
        // Business logic validation (Evaluate) should only run when user clicks Next/OK
        var newCanProceed = await step.ValidateAsync(_data);

        if (CanProceed != newCanProceed)
        {
            CanProceed = newCanProceed;
            RaiseStateChanged();
        }
    }
}