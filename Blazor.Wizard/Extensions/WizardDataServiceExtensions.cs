using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Extensions;

/// <summary>
/// Helper methods for storing and resolving services in wizard runtime data.
/// </summary>
public static class WizardDataServiceExtensions
{
    public static IWizardData AddService<TService>(this IWizardData data, TService service)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(service);

        data.Set(service);
        return data;
    }

    public static TService GetService<TService>(this IWizardData data)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.TryGet<TService>(out var service) && service is not null)
        {
            return service;
        }

        throw new InvalidOperationException(
            $"Service of type '{typeof(TService).Name}' was not found in wizard data. " +
            "Register it with AddService<TService>() before starting the wizard.");
    }

    public static bool TryGetService<TService>(this IWizardData data, out TService? service)
        where TService : class
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.TryGet<TService>(out service) && service is not null)
        {
            return true;
        }

        service = null;
        return false;
    }
}
