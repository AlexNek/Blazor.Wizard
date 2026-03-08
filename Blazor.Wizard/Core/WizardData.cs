using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Core;

public sealed class WizardData : IPersistableWizardData, IWizardContext
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

    /// <summary>
    /// Gets all stored data for serialization.
    /// </summary>
    public Dictionary<Type, object> GetAllData() => new(_data);

    /// <summary>
    /// Loads data from deserialized state.
    /// </summary>
    public void LoadData(Dictionary<Type, object> data)
    {
        if (data == null) return;
        _data.Clear();
        foreach (var kvp in data)
            _data[kvp.Key] = kvp.Value;
    }
}