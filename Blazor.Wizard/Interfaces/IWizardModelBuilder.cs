namespace Blazor.Wizard.Interfaces;

public interface IWizardModelBuilder<out TResult>
{
    TResult Build(IWizardData data);
}
