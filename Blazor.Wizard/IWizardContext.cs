namespace Blazor.Wizard;

public interface IWizardContext
{
    void Set<T>(T value);
    T Get<T>();
    bool TryGet<T>(out T value);
}
