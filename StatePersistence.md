# State Persistence Guide

This guide explains how to implement state persistence for Blazor.Wizard, allowing wizards to resume after page refresh or browser restart.

---

## Design Philosophy

- **Storage-agnostic** - Users implement their own storage (Memory, Database, File System, etc.)
- **SSR-compatible** - Works during prerendering without JavaScript
- **No JS dependencies in core** - Library uses only .NET APIs
- **Opt-in** - Wizards work without persistence by default
- **Type-safe** - Leverages System.Text.Json for serialization
- **Minimal** - Simple interfaces, maximum flexibility

---

## ⚠️ SSR Compatibility Warning

**JavaScript-based storage (like `localStorage`) is NOT available during SSR prerendering.**

This affects:
- Blazor Server during prerender
- Blazor Web App with Static SSR
- Any component rendered before the circuit becomes interactive

**Solution**: Use server-side storage (Memory, Database, File System) during SSR, then optionally sync to browser storage after hydration.

---

## Core Interfaces

### IWizardStateStorage

```csharp
namespace Blazor.Wizard.Interfaces;

/// <summary>
/// Defines storage operations for wizard state persistence.
/// Implement this interface to use Memory, Database, File System, or any custom storage.
/// </summary>
public interface IWizardStateStorage
{
    Task SaveAsync(string key, string state, CancellationToken ct = default);
    Task<string?> LoadAsync(string key, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
```

### WizardState

```csharp
namespace Blazor.Wizard.Core;

public sealed class WizardState
{
    public int CurrentStepIndex { get; set; }
    public Dictionary<string, string> SerializedData { get; set; } = new();
    public DateTime SavedAt { get; set; }
}
```

---

## Implementation Examples

### 1. Memory Storage (SSR-Safe, Recommended Default)

```csharp
using System.Collections.Concurrent;
using Blazor.Wizard.Interfaces;

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
```

**Pros**: Works everywhere including SSR prerender  
**Cons**: State lost on server restart (use scoped lifetime for per-user state)

### 2. ProtectedBrowserStorage (Interactive Only)

⚠️ **Not SSR-safe** - Only use after interactive rendering.

```csharp
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Blazor.Wizard.Interfaces;

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
```

### 3. Hybrid Storage (SSR-Safe with Browser Fallback) ⭐

**Recommended for production** - Works during SSR, upgrades to browser storage when available.

```csharp
using Blazor.Wizard.Interfaces;

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
                return await _browserStorage.LoadAsync(key, ct);
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
```

### 4. Database Storage (SSR-Safe, Persistent)

```csharp
using Microsoft.EntityFrameworkCore;
using Blazor.Wizard.Interfaces;

public sealed class DatabaseWizardStateStorage : IWizardStateStorage
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly string _userId;

    public DatabaseWizardStateStorage(IDbContextFactory<AppDbContext> contextFactory, string userId)
    {
        _contextFactory = contextFactory;
        _userId = userId;
    }

    public async Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var entity = await context.WizardStates
            .FirstOrDefaultAsync(x => x.UserId == _userId && x.Key == key, ct);

        if (entity == null)
        {
            entity = new WizardStateEntity { UserId = _userId, Key = key };
            context.WizardStates.Add(entity);
        }

        entity.State = state;
        entity.SavedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
    }

    public async Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var entity = await context.WizardStates
            .FirstOrDefaultAsync(x => x.UserId == _userId && x.Key == key, ct);
        return entity?.State;
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var entity = await context.WizardStates
            .FirstOrDefaultAsync(x => x.UserId == _userId && x.Key == key, ct);

        if (entity != null)
        {
            context.WizardStates.Remove(entity);
            await context.SaveChangesAsync(ct);
        }
    }
}

public class WizardStateEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}
```

### 5. File System Storage (SSR-Safe, Server Only)

```csharp
using Blazor.Wizard.Interfaces;

public sealed class FileSystemWizardStateStorage : IWizardStateStorage
{
    private readonly string _basePath;

    public FileSystemWizardStateStorage(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        await File.WriteAllTextAsync(GetFilePath(key), state, ct);
    }

    public async Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        var filePath = GetFilePath(key);
        return File.Exists(filePath) ? await File.ReadAllTextAsync(filePath, ct) : null;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        var filePath = GetFilePath(key);
        if (File.Exists(filePath)) File.Delete(filePath);
        return Task.CompletedTask;
    }

    private string GetFilePath(string key) => Path.Combine(_basePath, $"{key}.json");
}
```

---

## Storage Comparison

| Storage Type | SSR-Safe | Survives Restart | Multi-User | Use Case |
|--------------|----------|------------------|------------|----------|
| Memory | ✅ | ❌ | ✅ (scoped) | Development, temporary state |
| ProtectedBrowserStorage | ❌ | ✅ | ✅ | Interactive-only apps |
| **Hybrid** | ✅ | ✅ | ✅ | **Production (recommended)** |
| Database | ✅ | ✅ | ✅ | Enterprise, audit trail |
| File System | ✅ | ✅ | ✅ | Server-side apps |

---

## Extension Methods

```csharp
using System.Text.Json;
using Blazor.Wizard.Core;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

namespace Blazor.Wizard.Extensions;

public static class WizardPersistenceExtensions
{
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
            CurrentStepIndex = viewModel.Flow?.Index ?? 0,
            SavedAt = DateTime.UtcNow
        };

        var wizardData = (WizardData)(object)viewModel.Data;
        foreach (var kvp in wizardData.GetAllData())
        {
            var typeName = kvp.Key.AssemblyQualifiedName 
                ?? throw new InvalidOperationException($"Cannot serialize type {kvp.Key}");
            state.SerializedData[typeName] = JsonSerializer.Serialize(kvp.Value, kvp.Value.GetType());
        }

        await storage.SaveAsync(key, JsonSerializer.Serialize(state), ct);
    }

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
        if (string.IsNullOrEmpty(json)) return false;

        var state = JsonSerializer.Deserialize<WizardState>(json);
        if (state == null) return false;

        var dataDict = new Dictionary<Type, object>();
        foreach (var kvp in state.SerializedData)
        {
            var type = Type.GetType(kvp.Key);
            if (type == null) continue;

            var obj = JsonSerializer.Deserialize(kvp.Value, type);
            if (obj != null) dataDict[type] = obj;
        }

        var wizardData = (WizardData)(object)viewModel.Data;
        wizardData.LoadData(dataDict);

        if (viewModel.Flow != null)
            viewModel.Flow.Index = state.CurrentStepIndex;

        return true;
    }

    public static async Task ClearStateAsync(
        string key,
        IWizardStateStorage storage,
        CancellationToken ct = default)
    {
        await storage.RemoveAsync(key, ct);
    }
}
```

---

## Required WizardData Modifications

```csharp
// Blazor.Wizard/Core/WizardData.cs

public Dictionary<Type, object> GetAllData() => new(_data);

public void LoadData(Dictionary<Type, object> data)
{
    _data.Clear();
    foreach (var kvp in data)
        _data[kvp.Key] = kvp.Value;
}
```

---

## DI Registration

### Option 1: Memory Storage (Simple)

```csharp
builder.Services.AddScoped<IWizardStateStorage, MemoryWizardStateStorage>();
```

### Option 2: Hybrid Storage (Recommended)

```csharp
namespace Blazor.Wizard.Extensions;

public static class WizardStorageServiceCollectionExtensions
{
    public static IServiceCollection AddWizardStateStorage(this IServiceCollection services)
    {
        services.AddScoped<MemoryWizardStateStorage>();
        services.AddScoped<ProtectedLocalStorageWizardStateStorage>();
        
        // The hybrid implementation is the one that will be used when someone asks for IWizardStateStorage
        services.AddScoped<IWizardStateStorage, HybridWizardStateStorage>();

        return services;
    }
}
```

### Option 3: Database Storage

```csharp
builder.Services.AddScoped<IWizardStateStorage>(sp =>
{
    var contextFactory = sp.GetRequiredService<IDbContextFactory<AppDbContext>>();
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var userId = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
        ?? throw new InvalidOperationException("User not authenticated");
    return new DatabaseWizardStateStorage(contextFactory, userId);
});
```

---

## Usage Example

```csharp
@page "/wizard"
@inject IWizardStateStorage Storage

@code {
    private PersonWizardViewModel _viewModel = null!;
    private const string StateKey = "person-wizard-state";

    protected override void OnInitialized()
    {
        _viewModel = new PersonWizardViewModel(new PersonModelMapper());
        _viewModel.Initialize(GetStepFactories());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var restored = await _viewModel.LoadStateAsync(StateKey, Storage);
            if (!restored) await _viewModel.StartAsync();
            StateHasChanged();
        }
    }

    private async Task OnNextClick()
    {
        if (await _viewModel.NextAsync())
            await _viewModel.SaveStateAsync(StateKey, Storage);
    }

    private async Task OnFinishClick()
    {
        var result = await _viewModel.FinishAsync();
        if (result != null)
            await WizardPersistenceExtensions.ClearStateAsync(StateKey, Storage);
    }
}
```

---

## Testing

```csharp
[Fact]
public async Task SaveAndLoadState_PreservesWizardProgress()
{
    var storage = new MemoryWizardStateStorage();
    var viewModel = new PersonWizardViewModel(new PersonModelMapper());
    viewModel.Initialize(GetStepFactories());
    await viewModel.StartAsync();
    
    viewModel.Data.Set(new PersonModel { Name = "John", Age = 30 });
    await viewModel.NextAsync();
    await viewModel.SaveStateAsync("test-key", storage);

    var newViewModel = new PersonWizardViewModel(new PersonModelMapper());
    newViewModel.Initialize(GetStepFactories());
    var loaded = await newViewModel.LoadStateAsync("test-key", storage);

    Assert.True(loaded);
    Assert.True(newViewModel.Data.TryGet<PersonModel>(out var person));
    Assert.Equal("John", person.Name);
    Assert.Equal(1, newViewModel.Flow?.Index);
}
```

---

## Best Practices

1. **Use unique keys** - Include user ID: `$"{userId}-person-wizard"`
2. **Clear on completion** - Remove state after successful finish
3. **Handle expiration** - Don't restore very old states
4. **Validate loaded data** - Check deserialized models
5. **Use Hybrid storage** - Best for SSR compatibility
6. **Test thoroughly** - Verify all model types serialize correctly
