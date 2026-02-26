namespace Blazor.Wizard.Interfaces;

public interface IWizardData
{
    void Set<T>(T value);
    bool TryGet<T>(out T? value);
}