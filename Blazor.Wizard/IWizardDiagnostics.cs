namespace Blazor.Wizard;

public interface IWizardDiagnostics
{
    void StepCompleted(string stepName);

    void StepEntered(string stepName);

    void TransitionBlocked(string stepName, string reason);

    void ValidationExecuted(string stepName, bool isValid);

    void WizardCompleted(string finalStepName);

    void WizardStarted(string firstStepName);
}
