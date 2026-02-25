using Blazor.Wizard;
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
        public void StepChanged(string stepName)
        {
            _logger.Information("Wizard step changed: {StepName}", stepName);
        }
        public void ValidationExecuted(string stepName, bool isValid)
        {
            _logger.Information("Validation executed for step {StepName}: IsValid={IsValid}", stepName, isValid);
        }
        public void TransitionBlocked(string stepName, string reason)
        {
            _logger.Warning("Transition blocked at step {StepName}: Reason={Reason}", stepName, reason);
        }
    }
}
