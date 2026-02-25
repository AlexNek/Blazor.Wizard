using System;
using System.Collections.Generic;
using Blazor.Wizard.Demo.Components.Questionary;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryStepFactory
{
    private readonly Dictionary<QuestionaryStepId, Func<IServiceProvider, IQuestionaryStep>> _creators = new()
    {
        [QuestionaryStepId.Step1] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep1>(sp),
        [QuestionaryStepId.Step2] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep2>(sp),
        [QuestionaryStepId.Step3] = sp => ActivatorUtilities.CreateInstance<QuestionaryStep3>(sp),
        [QuestionaryStepId.Report] = sp => ActivatorUtilities.CreateInstance<QuestionaryReportStep>(sp),
    };

    private readonly IServiceProvider _serviceProvider;

    public QuestionaryStepFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IQuestionaryStep Create(QuestionaryStepId step)
    {
        if (!_creators.TryGetValue(step, out var creator))
            throw new NotSupportedException($"No step registered for {step}");
        return creator(_serviceProvider);
    }
}
