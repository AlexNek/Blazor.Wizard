using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryWizardViewModel : WizardViewModel<IWizardStep, WizardData, QuestionaryModel>
{
    public bool CanGoBack
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

    public Dictionary<string, object>? CurrentComponentParameters =>
        CurrentStep?.GetComponentParameters();

    public Type? CurrentComponentType
    {
        get
        {
            var step = CurrentStep;
            if (step == null)
            {
                return null;
            }

            return QuestionaryStepRegistry.GetByStepType(step.Id).ComponentType;
        }
    }

    public IWizardStep? CurrentStep
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

    public bool HasNextVisibleStep
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

    public QuestionaryWizardViewModel(IWizardDiagnostics? diagnostics = null)
        : base(new QuestionaryResultBuilder(), diagnostics)
    {
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        var effectiveFactories = stepFactories?.ToList();
        if (effectiveFactories == null || effectiveFactories.Count == 0)
        {
            effectiveFactories = QuestionaryStepRegistry.CreateStepFactories().ToList();
        }

        base.Initialize(effectiveFactories);
    }
}
