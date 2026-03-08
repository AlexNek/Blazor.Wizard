using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Persistence;

/// <summary>
/// Hybrid storage that works during SSR (using memory) and upgrades to browser storage when available.
/// Recommended for production use.
/// </summary>
public sealed class HybridWizardStateStorage : IWizardStateStorage
{
    private readonly ProtectedLocalStorageWizardStateStorage? _browserStorage;
    private readonly MemoryWizardStateStorage _memoryStorage;

    public HybridWizardStateStorage(
        ProtectedLocalStorageWizardStateStorage? browserStorage,
        MemoryWizardStateStorage memoryStorage)
    {
        _browserStorage = browserStorage;
        _memoryStorage = memoryStorage;
    }

    public async Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        if (_browserStorage != null)
        {
            try
            {
                await _browserStorage.SaveAsync(key, state, ct);
                return;
            }
            catch { /* Fall back to memory */ }
        }
        await _memoryStorage.SaveAsync(key, state, ct);
    }

    public async Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        if (_browserStorage != null)
        {
            try
            {
                var result = await _browserStorage.LoadAsync(key, ct);
                if (result != null)
                {
                    return result;
                }
            }
            catch { /* Fall back to memory */ }
        }
        return await _memoryStorage.LoadAsync(key, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (_browserStorage != null)
        {
            try
            {
                await _browserStorage.RemoveAsync(key, ct);
                return;
            }
            catch { /* Fall back to memory */ }
        }
        await _memoryStorage.RemoveAsync(key, ct);
    }
}
