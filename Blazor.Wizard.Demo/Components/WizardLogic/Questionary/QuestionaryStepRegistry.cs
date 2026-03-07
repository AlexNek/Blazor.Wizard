using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Components.Questionary;
using Blazor.Wizard.Demo.Models.Questionary;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public static class QuestionaryStepRegistry
{
    private static readonly QuestionaryResultBuilder _resultBuilder = new();

    private static readonly List<StepRegistration> _steps = new()
    {
        new(
            EQuestionaryStepId.Step1,
            typeof(QuestionaryStep1Model),
            () => new FormStepLogic<QuestionaryStep1Model>(typeof(QuestionaryStep1Model)),
            typeof(QuestionaryStep1)),
        new(
            EQuestionaryStepId.Step2,
            typeof(QuestionaryStep2Model),
            () => new FormStepLogic<QuestionaryStep2Model>(typeof(QuestionaryStep2Model)),
            typeof(QuestionaryStep2)),
        new(
            EQuestionaryStepId.Step3,
            typeof(QuestionaryStep3Model),
            () => new FormStepLogic<QuestionaryStep3Model>(typeof(QuestionaryStep3Model)),
            typeof(QuestionaryStep3)),
        new(
            EQuestionaryStepId.Report,
            typeof(QuestionaryModel),
            () => new ResultStepLogic<QuestionaryModel>(typeof(QuestionaryModel), data => _resultBuilder.Build(data)),
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

    public static StepRegistration GetByStepIdType(Type stepIdType)
    {
        var step = _steps.FirstOrDefault(s => s.StepIdType == stepIdType);
        if (step == null)
        {
            throw new InvalidOperationException(
                $"Step type '{stepIdType.Name}' is not registered in QuestionaryStepRegistry. " +
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
