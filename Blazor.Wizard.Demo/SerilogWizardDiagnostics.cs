using Blazor.Wizard;
using Blazor.Wizard.Interfaces;

using Serilog;

namespace Blazor.Wizard.Demo
{
    public class SerilogWizardDiagnostics : IWizardDiagnostics
    {
        private readonly Serilog.ILogger _logger;
        public SerilogWizardDiagnostics(Serilog.ILogger logger)
        {
            _logger = logger;
        }
        public void WizardStarted(string firstStepName)
        {
            _logger.Information("[WIZARD] Wizard started at step: {StepName}", firstStepName);
        }
        public void StepEntered(string stepName)
        {
            _logger.Information("[WIZARD] Entered step: {StepName}", stepName);
        }
        public void StepCompleted(string stepName)
        {
            _logger.Information("[WIZARD] Step completed: {StepName}", stepName);
        }
        public void ValidationExecuted(string stepName, bool isValid)
        {
            _logger.Information("[WIZARD] Validation on {StepName}: {Result}", stepName, isValid ? "PASSED" : "FAILED");
        }
        public void TransitionBlocked(string stepName, string reason)
        {
            _logger.Warning("[WIZARD] Blocked at {StepName}: {Reason}", stepName, reason);
        }
        public void WizardCompleted(string finalStepName)
        {
            _logger.Information("[WIZARD] Wizard completed successfully at step: {StepName}", finalStepName);
        }
    }
}
