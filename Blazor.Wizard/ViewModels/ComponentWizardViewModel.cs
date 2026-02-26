using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Obsolete;

namespace Blazor.Wizard.ViewModels;

public abstract class ComponentWizardViewModel<TResult> : WizardViewModel<IWizardStep, WizardData, TResult>
    where TResult : class
{
    protected ComponentWizardViewModel(
        IWizardModelBuilder<TResult> mapper,
        IWizardDiagnostics? diagnostics = null)
        : base(mapper, diagnostics)
    {
    }

    [Obsolete("Use constructor with IWizardModelBuilder<TResult> instead")]
    protected ComponentWizardViewModel(
        IWizardResultBuilder<TResult> resultBuilder,
        IWizardDiagnostics? diagnostics = null)
        : base(resultBuilder, diagnostics)
    {
    }

    public virtual IWizardStep? CurrentStep
    {
        get
        {
            if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count)
            {
                return null;
            }

            return Steps[Flow.Index];
        }
    }

    public virtual bool CanGoBack
    {
        get
        {
            if (Flow == null || Steps.Count == 0 || Flow.Index <= 0)
            {
                return false;
            }

            return Enumerable.Range(0, Flow.Index).Any(i => Steps[i].IsVisible);
        }
    }

    public virtual bool HasNextVisibleStep
    {
        get
        {
            if (Flow == null || Steps.Count == 0 || Flow.Index < 0 || Flow.Index >= Steps.Count - 1)
            {
                return false;
            }

            return Enumerable.Range(Flow.Index + 1, Steps.Count - Flow.Index - 1)
                .Any(i => Steps[i].IsVisible);
        }
    }

    public virtual Dictionary<string, object>? CurrentComponentParameters => CurrentStep?.GetComponentParameters();

    public virtual Type? CurrentComponentType
    {
        get
        {
            var step = CurrentStep;
            if (step == null)
            {
                return null;
            }

            return ResolveComponentType(step);
        }
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        var effectiveFactories = stepFactories?.ToList();
        if (effectiveFactories == null || effectiveFactories.Count == 0)
        {
            effectiveFactories = GetDefaultStepFactories().ToList();
        }

        base.Initialize(effectiveFactories);
    }

    protected abstract Type ResolveComponentType(IWizardStep step);

    protected abstract IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories();
}
