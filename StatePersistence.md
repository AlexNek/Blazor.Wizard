# State Persistence

Blazor.Wizard supports state persistence, allowing wizards to resume after page refresh or browser restart.

---

## Quick Start

### 1. Register Storage Service

```csharp
// Program.cs
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Persistence;

builder.Services.AddScoped<IWizardStateStorage, MemoryWizardStateStorage>();
```

### 2. Use in Your Component

```csharp
@page "/wizard"
@inject IWizardStateStorage Storage

@code {
    private PersonWizardViewModel _viewModel = null!;
    private const string StateKey = "person-wizard-state";

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

## Built-in Storage

The library includes three storage implementations:

### 1. Memory Storage

SSR-safe, works everywhere. State is lost on server restart.

```csharp
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.Persistence;

builder.Services.AddScoped<IWizardStateStorage, MemoryWizardStateStorage>();
```

**Use for:** Development, temporary state

### 2. ProtectedLocalStorage

Browser storage wrapper. Not SSR-safe - only works after interactive rendering.

```csharp
builder.Services.AddScoped<IWizardStateStorage, ProtectedLocalStorageWizardStateStorage>();
```

**Use for:** Interactive-only apps (WebAssembly, fully interactive Server)

### 3. Hybrid Storage (Recommended)

Combines memory (SSR-safe) with browser storage (persistent). Automatically switches based on rendering mode.

```csharp
builder.Services.AddScoped<MemoryWizardStateStorage>();
builder.Services.AddScoped<ProtectedLocalStorageWizardStateStorage>();
builder.Services.AddScoped<IWizardStateStorage, HybridWizardStateStorage>();
```

**Use for:** Production apps with SSR

**How it works:**
- During SSR prerendering → uses memory storage
- After interactive rendering → upgrades to browser storage
- Automatically falls back to memory if browser storage fails

---

## Custom Storage

Implement `IWizardStateStorage` for custom storage solutions:

```csharp
public interface IWizardStateStorage
{
    Task SaveAsync(string key, string state, CancellationToken ct = default);
    Task<string?> LoadAsync(string key, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
}
```

### Example: Database Storage

```csharp
public class DatabaseWizardStateStorage : IWizardStateStorage
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
            .FirstOrDefaultAsync(x => x.UserId == _userId && x.Key == key, ct) 
            ?? new WizardStateEntity { UserId = _userId, Key = key };
        
        entity.State = state;
        entity.SavedAt = DateTime.UtcNow;
        
        if (context.Entry(entity).State == EntityState.Detached)
            context.WizardStates.Add(entity);
            
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
        await context.WizardStates
            .Where(x => x.UserId == _userId && x.Key == key)
            .ExecuteDeleteAsync(ct);
    }
}
```

### Example: File System Storage

```csharp
public class FileSystemWizardStateStorage : IWizardStateStorage
{
    private readonly string _basePath;

    public FileSystemWizardStateStorage(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task SaveAsync(string key, string state, CancellationToken ct = default)
    {
        await File.WriteAllTextAsync(Path.Combine(_basePath, $"{key}.json"), state, ct);
    }

    public async Task<string?> LoadAsync(string key, CancellationToken ct = default)
    {
        var path = Path.Combine(_basePath, $"{key}.json");
        return File.Exists(path) ? await File.ReadAllTextAsync(path, ct) : null;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        var path = Path.Combine(_basePath, $"{key}.json");
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}
```

---

## Extension Methods

The library provides three extension methods:

### SaveStateAsync

Saves current wizard state to storage.

```csharp
await viewModel.SaveStateAsync("my-wizard-key", storage);
```

### LoadStateAsync

Loads wizard state from storage. Returns `true` if state was found and loaded.

```csharp
var restored = await viewModel.LoadStateAsync("my-wizard-key", storage);
if (!restored)
{
    // No saved state, start fresh
    await viewModel.StartAsync();
}
```

### ClearStateAsync

Removes wizard state from storage.

```csharp
await WizardPersistenceExtensions.ClearStateAsync("my-wizard-key", storage);
```

---

## Auto-Save Pattern

Save state automatically on every step change:

```csharp
protected override void OnInitialized()
{
    _viewModel = new PersonWizardViewModel(new PersonModelMapper());
    _viewModel.StateChanged += async () =>
    {
        await _viewModel.SaveStateAsync(StateKey, Storage);
        StateHasChanged();
    };
    _viewModel.Initialize(GetStepFactories());
}
```

---

## SSR Compatibility

⚠️ **Important:** JavaScript-based storage (like `localStorage`) is **not available during SSR prerendering**.

**Solutions:**

### Option 1: Memory Storage (Simple)
```csharp
builder.Services.AddScoped<IWizardStateStorage, MemoryWizardStateStorage>();
```

### Option 2: Hybrid Storage (Recommended)

Use the built-in hybrid storage that automatically switches between memory and browser storage.

```csharp
builder.Services.AddScoped<MemoryWizardStateStorage>();
builder.Services.AddScoped<ProtectedLocalStorageWizardStateStorage>();
builder.Services.AddScoped<IWizardStateStorage, HybridWizardStateStorage>();
```

### Option 3: Database/File System

Use server-side storage that works during SSR:
- ✅ Database storage
- ✅ File system storage

---

## Best Practices

### 1. Use Unique Keys

Include user ID to prevent conflicts:

```csharp
var stateKey = $"{userId}-person-wizard";
```

### 2. Clear on Completion

Remove state after successful wizard completion:

```csharp
var result = await _viewModel.FinishAsync();
if (result != null)
{
    await WizardPersistenceExtensions.ClearStateAsync(StateKey, Storage);
}
```

### 3. Handle Expiration

Don't restore very old states:

```csharp
public async Task<bool> LoadStateWithExpirationAsync(
    string key, 
    IWizardStateStorage storage, 
    TimeSpan maxAge)
{
    var json = await storage.LoadAsync(key);
    if (string.IsNullOrEmpty(json)) return false;

    var state = JsonSerializer.Deserialize<WizardState>(json);
    if (state == null || DateTime.UtcNow - state.SavedAt > maxAge)
    {
        await storage.RemoveAsync(key);
        return false;
    }

    return await _viewModel.LoadStateAsync(key, storage);
}
```

### 4. Validate Loaded Data

Check if deserialized models are valid before using them.

### 5. Test Serialization

Ensure all your model types are JSON-serializable:

```csharp
[Fact]
public void AllModels_AreSerializable()
{
    var model = new PersonModel { Name = "Test", Age = 30 };
    var json = JsonSerializer.Serialize(model);
    var deserialized = JsonSerializer.Deserialize<PersonModel>(json);
    
    Assert.NotNull(deserialized);
    Assert.Equal(model.Name, deserialized.Name);
}
```

---

## Storage Comparison

| Storage | SSR-Safe | Survives Restart | Multi-User | Use Case |
|---------|----------|------------------|------------|----------|
| Memory | ✅ | ❌ | ✅ (scoped) | Development, temporary |
| **Hybrid** | ✅ | ✅ | ✅ | **Production (recommended)** |
| Database | ✅ | ✅ | ✅ | Enterprise, audit trail |
| File System | ✅ | ✅ | ✅ | Server-side apps |
| Browser Storage | ❌ | ✅ | ✅ | Interactive-only apps |

---

## Troubleshooting

### State Not Restoring

1. Check if `LoadStateAsync` returns `true`
2. Verify storage key is consistent
3. Ensure models are JSON-serializable
4. Check for exceptions in storage implementation

### State Lost on Refresh

1. Verify storage survives restart (Memory storage doesn't)
2. Check if state is being cleared prematurely
3. Ensure storage key includes user identifier

### SSR Errors

If you get JavaScript errors during prerendering:
- Use `MemoryWizardStateStorage` instead of browser storage
- Implement hybrid storage with fallback
- Only access browser storage after `OnAfterRenderAsync`

---

## Demo

The demo project (`/wizard-persistence-demo`) showcases the built-in hybrid storage:

- **Hybrid storage** - SSR-safe with browser persistence
- **Auto-save** - State saved on every step change
- **Auto-restore** - Wizard resumes after page refresh
- **Clear state** - Manual state cleanup

**Try it:**
1. Start the wizard and fill in some steps
2. Refresh the page → wizard resumes where you left off
3. Complete the wizard → state is automatically cleared

**Source code:**
- Demo page: `Blazor.Wizard.Demo/Pages/WizardPersistenceDemo.razor`
- DI registration: `Blazor.Wizard.Demo/Program.cs`
