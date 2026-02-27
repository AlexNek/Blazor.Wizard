using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.DemoDevEx;

public static class StartupWizardDiagnostics
{
    public static IWizardDiagnostics Create()
    {
        return new NLogWizardDiagnostics();
    }
}
