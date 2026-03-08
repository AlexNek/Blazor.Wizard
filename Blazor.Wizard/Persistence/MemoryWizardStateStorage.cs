using System.Collections.Concurrent;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Persistence;

/// <summary>
/// In-memory wizard state storage. SSR-safe and works everywhere.
/// State is lost on server restart. Use scoped lifetime for per-user state.
/// </summary>
public sealed class MemoryWizardStateStorage : IWizardStateStorage
{
    private readonly ConcurrentDictionary<string, string> _store = new();

    public Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        _store[key] = state;
        return Task.CompletedTask;
    }

    public Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        _store.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
