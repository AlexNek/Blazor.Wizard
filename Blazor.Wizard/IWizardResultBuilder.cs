namespace Blazor.Wizard;

public interface IWizardResultBuilder<TResult>
{
    TResult Build(IWizardData data);
}