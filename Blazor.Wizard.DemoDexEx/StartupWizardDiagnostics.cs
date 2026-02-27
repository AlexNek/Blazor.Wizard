using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.DemoDevEx;

public static class StartupWizardDiagnostics
{
    public static IWizardDiagnostics Create()
    {
        // For now, return a no-op diagnostics implementation
        // You can replace this with SerilogWizardDiagnostics if needed
        return new NoOpWizardDiagnostics();
    }

    private class NoOpWizardDiagnostics : IWizardDiagnostics
    {
        public void StepEntered(string stepName) { }
        public void StepCompleted(string stepName) { }
        public void ValidationExecuted(string stepName, bool isValid) { }
        public void TransitionBlocked(string stepName, string reason) { }
        public void WizardStarted(string firstStepName) { }
        public void WizardCompleted(string finalStepName) { }
    }
}
