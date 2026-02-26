namespace Blazor.Wizard.Interfaces;

public interface IWizardStepFactory
{
    IWizardStep CreateStep(Type stepType);
}