using Blazor.Wizard.Demo.Components.Questionary;
using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public record StepRegistration(EQuestionaryStepId Id, object? Model, Type ComponentType);

public static class QuestionaryStepRegistry
{
    private static readonly List<StepRegistration> _steps = new()
                                                                {
                                                                    new(
                                                                        EQuestionaryStepId.Step1,
                                                                        new QuestionaryStep1Model(),
                                                                        typeof(QuestionaryStep1)),
                                                                    new(
                                                                        EQuestionaryStepId.Step2,
                                                                        new QuestionaryStep2Model(),
                                                                        typeof(QuestionaryStep2)),
                                                                    new(
                                                                        EQuestionaryStepId.Step3,
                                                                        new QuestionaryStep3Model(),
                                                                        typeof(QuestionaryStep3)),
                                                                    new(
                                                                        EQuestionaryStepId.Report,
                                                                        null,
                                                                        typeof(QuestionaryReportStep))
                                                                };

    public static IReadOnlyList<StepRegistration> Steps => _steps;

    static QuestionaryStepRegistry()
    {
        ValidateRegistrations();
    }

    public static Type GetComponentType(EQuestionaryStepId stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step == null)
        {
            throw new InvalidOperationException(
                $"Step '{stepId}' is not registered in QuestionaryStepRegistry. " +
                $"Add it to the _steps list in QuestionaryStepRegistry.cs");
        }

        return step.ComponentType;
    }

    private static void ValidateRegistrations()
    {
        var allSteps = Enum.GetValues<EQuestionaryStepId>();
        var registeredSteps = _steps.Select(s => s.Id).ToHashSet();
        var missingSteps = allSteps.Except(registeredSteps).ToList();

        if (missingSteps.Any())
        {
            throw new InvalidOperationException(
                $"Missing step registrations in QuestionaryStepRegistry: {string.Join(", ", missingSteps)}. "
                +
                $"Add them to the _steps list in QuestionaryStepRegistry.cs");
        }
    }
}
