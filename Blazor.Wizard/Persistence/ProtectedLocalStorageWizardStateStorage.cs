using Blazor.Wizard.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Blazor.Wizard.Persistence;

/// <summary>
/// Browser storage using ProtectedLocalStorage. Not SSR-safe - only works after interactive rendering.
/// </summary>
public sealed class ProtectedLocalStorageWizardStateStorage : IWizardStateStorage
{
    private readonly ProtectedLocalStorage _localStorage;

    public ProtectedLocalStorageWizardStateStorage(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        await _localStorage.SetAsync(key, state);
    }

    public async Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        var result = await _localStorage.GetAsync<string>(key);
        return result.Success ? result.Value : null;
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _localStorage.DeleteAsync(key);
    }
}
