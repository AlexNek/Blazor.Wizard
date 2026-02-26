using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Core;

public sealed class WizardData : IWizardData, IWizardContext
{
    private readonly Dictionary<Type, object> _data = new();

    public void Set<T>(T value)
    {
        Type type = typeof(T);
        _data[type] = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void Set(object value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        _data[value.GetType()] = value;
    }

    public T Get<T>()
    {
        if (_data.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }
        throw new KeyNotFoundException($"Model of type {typeof(T).Name} not found in context.");
    }

    public bool TryGet<T>(out T value)
    {
        if (_data.TryGetValue(typeof(T), out var obj))
        {
            value = (T)obj;
            return true;
        }
        value = default;
        return false;
    }
}