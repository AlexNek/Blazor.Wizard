namespace Blazor.Wizard;

public interface IWizardData
{
    void Set<T>(T value);
    bool TryGet<T>(out T? value);
}