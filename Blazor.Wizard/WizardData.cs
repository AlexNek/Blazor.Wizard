namespace Blazor.Wizard;

public sealed class WizardData : IWizardData
{
    private readonly Dictionary<Type, object> _data = new();

    public void Set<T>(T value)
    {
        _data[typeof(T)] = value!;
    }

    public bool TryGet<T>(out T? value)
    {
        if (_data.TryGetValue(typeof(T), out var v))
        {
            value = (T)v;
            return true;
        }

        value = default;
        return false;
    }
}