using Blazor.Wizard.Demo.Components.Person;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed class PersonWizardDefinition
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IReadOnlyList<PersonStepDefinition> _steps;

    public PersonWizardDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _steps =
            [
                new(
                    EPersonStepId.PersonInfo,
                    typeof(PersonInfoStepLogic),
                    typeof(PersonInfoForm),
                    sp => ActivatorUtilities.CreateInstance<PersonInfoStepLogic>(sp)),
                new(
                    EPersonStepId.Address,
                    typeof(AddressStepLogic),
                    typeof(AddressForm),
                    sp => ActivatorUtilities.CreateInstance<AddressStepLogic>(sp)),
                new(
                    EPersonStepId.PensionInfo,
                    typeof(PensionInfoStepLogic),
                    typeof(PensionInfoForm),
                    sp => ActivatorUtilities.CreateInstance<PensionInfoStepLogic>(sp)),
                new(
                    EPersonStepId.Summary,
                    typeof(SummaryStepLogic),
                    typeof(SummaryView),
                    sp => ActivatorUtilities.CreateInstance<SummaryStepLogic>(sp))
            ];

        ValidateRegistrations();
    }

    public IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps
            .Select(step => new Func<IWizardStep>(() => step.StepFactory(_serviceProvider)))
            .ToList();
    }

    public Type ResolveComponentType(Type stepIdType)
    {
        var step = _steps.FirstOrDefault(s => s.StepIdType == stepIdType);
        if (step == null)
        {
            throw new InvalidOperationException(
                $"Step type '{stepIdType.Name}' is not registered in PersonWizardDefinition.");
        }

        return step.ComponentType;
    }

    private void ValidateRegistrations()
    {
        var allSteps = Enum.GetValues<EPersonStepId>();
        var registeredSteps = _steps.Select(s => s.Id).ToHashSet();
        var missingSteps = allSteps.Except(registeredSteps).ToList();

        if (missingSteps.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing step registrations in PersonWizardDefinition: {string.Join(", ", missingSteps)}.");
        }
    }
}
