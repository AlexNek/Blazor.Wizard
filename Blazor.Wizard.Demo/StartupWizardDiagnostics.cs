using Blazor.Wizard;
using Serilog;

namespace Blazor.Wizard.Demo
{
    public static class StartupWizardDiagnostics
    {
        public static SerilogWizardDiagnostics Create()
        {
            return new SerilogWizardDiagnostics(Log.Logger);
        }
    }
}
