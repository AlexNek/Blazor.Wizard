using Blazor.Wizard.Demo.Components.Questionary;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryStepFactory
{
    private readonly Dictionary<EQuestionaryStepId, Func<IServiceProvider, IQuestionaryStep>>
        _creators = new()
                        {
                            [EQuestionaryStepId.Step1] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep1>(sp),
                            [EQuestionaryStepId.Step2] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep2>(sp),
                            [EQuestionaryStepId.Step3] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep3>(sp),
                            [EQuestionaryStepId.Report] = sp => ActivatorUtilities.CreateInstance<QuestionaryReportStep>(sp)
                        };

    private readonly IServiceProvider _serviceProvider;

    public QuestionaryStepFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IQuestionaryStep Create(EQuestionaryStepId step)
    {
        if (!_creators.TryGetValue(step, out var creator))
        {
            throw new NotSupportedException($"No step registered for {step}");
        }

        return creator(_serviceProvider);
    }
}
