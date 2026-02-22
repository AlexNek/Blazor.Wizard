namespace Blazor.Wizard;

public interface IWizardStepFactory
{
    IWizardStep CreateStep(Type stepType);
}