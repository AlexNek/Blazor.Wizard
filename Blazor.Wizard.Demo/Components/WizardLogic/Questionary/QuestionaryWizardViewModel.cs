using Blazor.Wizard.Demo.Models;

using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class QuestionaryWizardViewModel
{
    private readonly QuestionaryModel _mainModel;
    private readonly QuestionaryResultBuilder _resultBuilder = new();
    private readonly WizardData _wizardData = new();

    public EditContext CurrentEditContext => Engine.CurrentEditContext;

    public int CurrentIndex => Engine.CurrentIndex;

    public WizardStepState CurrentStep => Engine.CurrentStep;

    public object CurrentStepModel
    {
        get
        {
            if (CurrentStep.Name == EQuestionaryStepId.Report.ToString())
            {
                return _resultBuilder.Build(_wizardData);
            }
            return CurrentStep.Model;
        }
    }

    public WizardEngine Engine { get; }

    public bool IsFirstStep => Engine.IsFirstStep;

    public bool IsLastStep => Engine.IsLastStep;

    public IReadOnlyList<WizardStepState> Steps => Engine.Steps;

    public IWizardData WizardData => _wizardData;

    public QuestionaryWizardViewModel(
        QuestionaryValidator validator,
        IWizardDiagnostics diagnostics,
        QuestionaryModel mainModel)
    {
        _mainModel = mainModel;
        
        var stepStates = QuestionaryStepRegistry.Steps
            .Select(s => new WizardStepState(s.Model ?? _mainModel, s.Id.ToString()))
            .ToList();

        Engine = new WizardEngine(stepStates, validator, _wizardData, diagnostics);
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
