using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Wizard.Extensions;


public static class WizardServiceRegistationExtensions
{
    /// <summary>
    /// Adds the Wizard State Storage services to the service collection.
    /// Registers Memory, ProtectedLocalStorage, and Hybrid implementations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWizardStateStorage(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register concrete implementations
        services.AddScoped<MemoryWizardStateStorage>();
        services.AddScoped<ProtectedLocalStorageWizardStateStorage>();

        // Register the interface mapped to the Hybrid implementation
        services.AddScoped<IWizardStateStorage, HybridWizardStateStorage>();

        return services;
    }
}