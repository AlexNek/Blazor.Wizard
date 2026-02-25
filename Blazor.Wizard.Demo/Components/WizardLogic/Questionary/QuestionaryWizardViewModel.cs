using System;
using System.Collections.Generic;
using Blazor.Wizard;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryWizardViewModel
{
    public EditContext CurrentEditContext => Engine.CurrentEditContext;
    public int CurrentIndex => Engine.CurrentIndex;
    public WizardStepState CurrentStep => Engine.CurrentStep;
    public WizardEngine Engine { get; }
    public bool IsFirstStep => Engine.IsFirstStep;
    public bool IsLastStep => Engine.IsLastStep;
    public IReadOnlyList<WizardStepState> Steps => Engine.Steps;

    private readonly QuestionaryStepFactory _stepFactory;
    private readonly QuestionaryModel _mainModel;
    private readonly Blazor.Wizard.WizardData _wizardData = new();

    public IWizardData WizardData => _wizardData;

    public QuestionaryWizardViewModel(QuestionaryValidator validator, IWizardDiagnostics diagnostics, QuestionaryStepFactory stepFactory, QuestionaryModel mainModel)
    {
        _stepFactory = stepFactory;
        _mainModel = mainModel;
        var step1 = new QuestionaryStep1Model();
        var step2 = new QuestionaryStep2Model();
        var step3 = new QuestionaryStep3Model();
        // Register the same instances in WizardData and WizardStepState
        Engine = new WizardEngine(new List<WizardStepState>
        {
            new WizardStepState(step1, EQuestionaryStepId.Step1.ToString()),
            new WizardStepState(step2, EQuestionaryStepId.Step2.ToString()),
            new WizardStepState(step3, EQuestionaryStepId.Step3.ToString()),
            new WizardStepState(_mainModel, EQuestionaryStepId.Report.ToString())
        }, validator, _wizardData, diagnostics);
        _wizardData.Set(step1);
        _wizardData.Set(step2);
        _wizardData.Set(step3);
    }

    public WizardDebugSnapshot CreateSnapshot()
    {
        return Engine.CreateSnapshot();
    }

    public void MoveBack()
    {
        Engine.MoveBack();
    }

    public void MoveNext()
    {
        Engine.MoveNext();
    }

    public WizardTransitionState TryProceed()
    {
        return Engine.TryProceed();
    }
}
