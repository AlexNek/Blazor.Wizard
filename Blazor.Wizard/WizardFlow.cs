namespace Blazor.Wizard;

public sealed class WizardFlow<TStep> where TStep : notnull
{
    public event Action? StateChanged;
    private readonly IWizardData _data;
    private readonly Dictionary<TStep, IFlowStepAdapter> _stepAdapters = new();
    private readonly List<IWizardStep> _steps = new();
    public TStep Current { get; private set; } = default!;
    public IWizardData Data => _data;

    public int Index { get; set; }

    public WizardFlow(IWizardData data)
    {
        _data = data;
    }

    public void Add(IWizardStep step)
    {
        _steps.Add(step);
    }

    public async Task NextAsync()
    {
        if (Index < 0 || Index >= _steps.Count)
        {
            return;
        }

        var step = _steps[Index];
        // Adapter: check if adapter allows leaving
        if (Current is not null && _stepAdapters.TryGetValue(Current, out var leaveAdapter))
        {
            if (!await leaveAdapter.CanLeaveAsync())
            {
                return;
            }
        }

        if (!await step.ValidateAsync(_data))
        {
            return;
        }

        await step.BeforeLeaveAsync(_data);
        // Find next visible step
        var nextIndex = Index + 1;
        while (nextIndex < _steps.Count && !_steps[nextIndex].IsVisible) nextIndex++;
        if (nextIndex < _steps.Count)
        {
            Index = nextIndex;
            Current = GetStepId(_steps[Index]);
            // Adapter: notify adapter on entering next step
            if (Current is not null && _stepAdapters.TryGetValue(Current, out var nextAdapter))
            {
                await nextAdapter.OnEnterAsync();
            }

            await _steps[Index].EnterAsync(_data);
            StateChanged?.Invoke();
        }
    }

    // PrevAsync: navigate to previous visible step
    public async Task PrevAsync()
    {
        if (Index <= 0 || Index >= _steps.Count)
        {
            return;
        }

        var step = _steps[Index];
        // Adapter: check if adapter allows leaving
        if (Current is not null && _stepAdapters.TryGetValue(Current, out var leaveAdapter))
        {
            if (!await leaveAdapter.CanLeaveAsync())
            {
                return;
            }
        }

        await step.BeforeLeaveAsync(_data);
        // Find previous visible step
        var prevIndex = Index - 1;
        while (prevIndex >= 0 && !_steps[prevIndex].IsVisible) prevIndex--;
        if (prevIndex >= 0)
        {
            Index = prevIndex;
            Current = GetStepId(_steps[Index]);
            // Adapter: notify adapter on entering previous step
            if (Current is not null && _stepAdapters.TryGetValue(Current, out var prevAdapter))
            {
                await prevAdapter.OnEnterAsync();
            }

            await _steps[Index].EnterAsync(_data);
            StateChanged?.Invoke();
        }
    }

    public void Register(TStep id, IFlowStepAdapter adapter)
    {
        _stepAdapters[id] = adapter;
    }

    public async Task StartAsync()
    {
        Index = 0;
        if (_steps.Count == 0)
        {
            return;
        }

        await _steps[0].EnterAsync(_data);
        StateChanged?.Invoke();
    }

    // Helper to extract step id
    private TStep GetStepId(IWizardStep step)
    {
        if (step is IIdentifiableStep<TStep> identifiable)
        {
            return identifiable.Id;
        }

        return default!;
    }
}