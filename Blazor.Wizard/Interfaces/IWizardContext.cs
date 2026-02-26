namespace Blazor.Wizard.Interfaces;

public interface IWizardContext
{
    void Set<T>(T value);
    void Set(object value);
    T Get<T>();
    bool TryGet<T>(out T value);
}
