using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Core;

public class WizardStepFactory : IWizardStepFactory
{
    private readonly Dictionary<Type, Func<IWizardStep>> _registry = new();

    public IWizardStep CreateStep(Type stepType)
    {
        if (!_registry.TryGetValue(stepType, out var creator))
        {
            throw new InvalidOperationException($"No step registered for type '{stepType.Name}'.");
        }

        return creator();
    }

    public void Register(Type stepType, Func<IWizardStep> creator)
    {
        if (stepType == null)
        {
            throw new ArgumentNullException(nameof(stepType));
        }

        if (creator == null)
        {
            throw new ArgumentNullException(nameof(creator));
        }

        _registry[stepType] = creator;
    }
}