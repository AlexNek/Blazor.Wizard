namespace Blazor.Wizard.Interfaces;

public interface IWizardResultBuilder<TResult>
{
    TResult Build(IWizardData data);
}