namespace Blazor.Wizard.Interfaces;

public interface IWizardModelSplitter<TResult>
{
    void Split(TResult result, IWizardData data);
}
