using Blazor.Wizard.Demo.Components.Questionary;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public static class QuestionaryStepRegistry
{
    private static readonly List<StepRegistration> _steps = new()
    {
        new(
            EQuestionaryStepId.Step1,
            typeof(QuestionaryStep1Logic),
            () => new QuestionaryStep1Logic(),
            typeof(QuestionaryStep1)),
        new(
            EQuestionaryStepId.Step2,
            typeof(QuestionaryStep2Logic),
            () => new QuestionaryStep2Logic(),
            typeof(QuestionaryStep2)),
        new(
            EQuestionaryStepId.Step3,
            typeof(QuestionaryStep3Logic),
            () => new QuestionaryStep3Logic(),
            typeof(QuestionaryStep3)),
        new(
            EQuestionaryStepId.Report,
            typeof(QuestionaryReportStepLogic),
            () => new QuestionaryReportStepLogic(new QuestionaryResultBuilder()),
            typeof(QuestionaryReportStep))
    };

    public static IReadOnlyList<StepRegistration> Steps => _steps;

    static QuestionaryStepRegistry()
    {
        ValidateRegistrations();
    }

    public static IReadOnlyList<Func<IWizardStep>> CreateStepFactories()
    {
        return _steps.Select(step => step.StepFactory).ToList();
    }

    public static StepRegistration GetByStepType(Type stepType)
    {
        var step = _steps.FirstOrDefault(s => s.StepType == stepType);
        if (step == null)
        {
            throw new InvalidOperationException(
                $"Step type '{stepType.Name}' is not registered in QuestionaryStepRegistry. " +
                "Add it to the _steps list in QuestionaryStepRegistry.cs");
        }

        return step;
    }

    private static void ValidateRegistrations()
    {
        var allSteps = Enum.GetValues<EQuestionaryStepId>();
        var registeredSteps = _steps.Select(s => s.Id).ToHashSet();
        var missingSteps = allSteps.Except(registeredSteps).ToList();

        if (missingSteps.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing step registrations in QuestionaryStepRegistry: {string.Join(", ", missingSteps)}. " +
                "Add them to the _steps list in QuestionaryStepRegistry.cs");
        }
    }
}
