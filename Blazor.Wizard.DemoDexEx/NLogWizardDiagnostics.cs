using Blazor.Wizard.Interfaces;
using NLog;

namespace Blazor.Wizard.DemoDevEx;

public class NLogWizardDiagnostics : IWizardDiagnostics
{
    private static readonly NLog.ILogger Logger = LogManager.GetLogger("WizardDiagnostics");

    public void WizardStarted(string firstStepName)
    {
        Logger.Info("[WIZARD] Wizard started at step: {0}", firstStepName);
    }

    public void StepEntered(string stepName)
    {
        Logger.Info("[WIZARD] Entered step: {0}", stepName);
    }

    public void StepCompleted(string stepName)
    {
        Logger.Info("[WIZARD] Step completed: {0}", stepName);
    }

    public void ValidationExecuted(string stepName, bool isValid)
    {
        Logger.Info("[WIZARD] Validation on {0}: {1}", stepName, isValid ? "PASSED" : "FAILED");
    }

    public void TransitionBlocked(string stepName, string reason)
    {
        Logger.Warn("[WIZARD] Blocked at {0}: {1}", stepName, reason);
    }

    public void WizardCompleted(string finalStepName)
    {
        Logger.Info("[WIZARD] Wizard completed successfully at step: {0}", finalStepName);
    }
}
