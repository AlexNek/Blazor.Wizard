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
    /// <returns>True if state was loaded successfully, false otherwise</returns>
    public static async Task<bool> LoadStateAsync<TStep, TData, TResult>(
        this WizardViewModel<TStep, TData, TResult> viewModel,
        string key,
        IWizardStateStorage storage,
        CancellationToken ct = default)
        where TStep : IWizardStep
        where TData : IWizardData, new()
        where TResult : class
    {
        var json = await storage.LoadAsync(key, ct);
        if (string.IsNullOrEmpty(json))
        {
            return false;
        }

        var state = JsonSerializer.Deserialize<WizardState>(json);
        if (state == null)
        {
            return false;
        }

        // Only update data models from storage, leave everything else untouched
        var wizardData = (WizardData)(object)viewModel.Data;
        foreach (var kvp in state.SerializedData)
        {
            var type = Type.GetType(kvp.Key);
            if (type == null)
            {
                continue;
            }

            var obj = JsonSerializer.Deserialize(kvp.Value, type);
            if (obj != null)
            {
                wizardData.Set(obj);
            }
        }

        if (viewModel.Flow != null)
        {
            viewModel.Flow.Index = state.CurrentStepIndex;
        }

        return true;
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
        where TData : IWizardData, new()
        where TResult : class
    {
        var state = new WizardState
                        {
                            CurrentStepIndex = viewModel.Flow?.Index ?? 0, SavedAt = DateTime.UtcNow
                        };

        var wizardData = (WizardData)(object)viewModel.Data;
        foreach (var kvp in wizardData.GetAllData())
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
