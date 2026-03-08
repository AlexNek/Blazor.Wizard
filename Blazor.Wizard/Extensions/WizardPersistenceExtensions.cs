using System.Text.Json;

using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

namespace Blazor.Wizard.Extensions;

/// <summary>
/// Extension methods for wizard state persistence.
/// </summary>
public static class WizardPersistenceExtensions
{
    /// <summary>
    /// Removes wizard state from storage.
    /// </summary>
    public static async Task ClearStateAsync(
        string key,
        IWizardStateStorage storage,
        CancellationToken ct = default)
    {
        await storage.RemoveAsync(key, ct);
    }

    /// <summary>
    /// Loads wizard state from storage and restores the wizard.
    /// </summary>
    /// <returns>Saved step index if state was loaded, -1 otherwise</returns>
    public static async Task<int> LoadStateAsync<TStep, TData, TResult>(
        this WizardViewModel<TStep, TData, TResult> viewModel,
        string key,
        IWizardStateStorage storage,
        CancellationToken ct = default)
        where TStep : IWizardStep
        where TData : IWizardData, IPersistableWizardData, new()
        where TResult : class
    {
        var json = await storage.LoadAsync(key, ct);
        if (string.IsNullOrEmpty(json))
        {
            return -1;
        }

        var state = JsonSerializer.Deserialize<WizardState>(json);
        if (state == null)
        {
            return -1;
        }

        // Only update data models from storage, leave everything else untouched
        foreach (var kvp in state.SerializedData)
        {
            var type = Type.GetType(kvp.Key);
            if (type == null || !typeof(IWizardDataModel).IsAssignableFrom(type))
            {
                continue;
            }

            var obj = JsonSerializer.Deserialize(kvp.Value, type);
            if (obj != null)
            {
                var setMethod = typeof(IWizardData).GetMethod(nameof(IWizardData.Set))?.MakeGenericMethod(type);
                setMethod?.Invoke(viewModel.Data, new[] { obj });
            }
        }

        return state.CurrentStepIndex;
    }

    /// <summary>
    /// Saves the current wizard state to storage.
    /// </summary>
    public static async Task SaveStateAsync<TStep, TData, TResult>(
        this WizardViewModel<TStep, TData, TResult> viewModel,
        string key,
        IWizardStateStorage storage,
        CancellationToken ct = default)
        where TStep : IWizardStep
        where TData : IWizardData, IPersistableWizardData, new()
        where TResult : class
    {
        var state = new WizardState
                        {
                            CurrentStepIndex = viewModel.Flow?.Index ?? 0, SavedAt = DateTime.UtcNow
                        };

        foreach (var kvp in viewModel.Data.GetAllData())
        {
            // Only serialize types that implement IWizardDataModel
            if (!typeof(IWizardDataModel).IsAssignableFrom(kvp.Key))
            {
                continue;
            }

            var typeName = kvp.Key.AssemblyQualifiedName
                           ?? throw new InvalidOperationException(
                               $"Cannot serialize type {kvp.Key}");
            state.SerializedData[typeName] =
                JsonSerializer.Serialize(kvp.Value, kvp.Value.GetType());
        }

        await storage.SaveAsync(key, JsonSerializer.Serialize(state), ct);
    }
}
