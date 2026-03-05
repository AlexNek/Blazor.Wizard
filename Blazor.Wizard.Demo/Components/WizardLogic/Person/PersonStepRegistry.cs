using Blazor.Wizard.Demo.Components.Person;
using Blazor.Wizard.Demo.Services.Animation;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public static class PersonStepRegistry
{
    private static readonly List<PersonStepRegistration> _steps = new()
    {
        new(
            EPersonStepId.PersonInfo,
            typeof(PersonInfoStepLogic),
            typeof(PersonInfoForm),
            ctx => new PersonInfoStepLogic("Demo value from factory", ctx.AnimationService)),
        new(
            EPersonStepId.Address,
            typeof(AddressStepLogic),
            typeof(AddressForm),
            _ => new AddressStepLogic()),
        new(
            EPersonStepId.PensionInfo,
            typeof(PensionInfoStepLogic),
            typeof(PensionInfoForm),
            _ => new PensionInfoStepLogic()),
        new(
            EPersonStepId.Summary,
            typeof(SummaryStepLogic),
            typeof(SummaryView),
            _ => new SummaryStepLogic())
    };

    public static IReadOnlyList<PersonStepRegistration> Steps => _steps;

    static PersonStepRegistry()
    {
        ValidateRegistrations();
    }

    public static IReadOnlyList<Func<IWizardStep>> CreateStepFactories(IWizardAnimationService animationService)
    {
        ArgumentNullException.ThrowIfNull(animationService);

        var context = new PersonStepFactoryContext(animationService);
        return _steps.Select(step => new Func<IWizardStep>(() => step.StepFactory(context))).ToList();
    }

    public static PersonStepRegistration GetByStepIdType(Type stepIdType)
    {
        var step = _steps.FirstOrDefault(s => s.StepIdType == stepIdType);
        if (step == null)
        {
            throw new InvalidOperationException(
                $"Step type '{stepIdType.Name}' is not registered in PersonStepRegistry. " +
                "Add it to the _steps list in PersonStepRegistry.cs");
        }

        return step;
    }

    private static void ValidateRegistrations()
    {
        var allSteps = Enum.GetValues<EPersonStepId>();
        var registeredSteps = _steps.Select(s => s.Id).ToHashSet();
        var missingSteps = allSteps.Except(registeredSteps).ToList();

        if (missingSteps.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing step registrations in PersonStepRegistry: {string.Join(", ", missingSteps)}. " +
                "Add them to the _steps list in PersonStepRegistry.cs");
        }
    }
}
