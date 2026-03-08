namespace Blazor.Wizard.Interfaces;

/// <summary>
/// Defines storage operations for wizard state persistence.
/// Implement this interface to use Memory, Database, File System, or any custom storage.
/// </summary>
public interface IWizardStateStorage
{
    /// <summary>
    /// Saves wizard state to storage.
    /// </summary>
    /// <param name="key">Unique identifier for the wizard state</param>
    /// <param name="state">Serialized wizard state</param>
    /// <param name="ct">Cancellation token</param>
    Task SaveAsync(string key, string state, CancellationToken ct = default);

    /// <summary>
    /// Loads wizard state from storage.
    /// </summary>
    /// <param name="key">Unique identifier for the wizard state</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Serialized wizard state or null if not found</returns>
    Task<string?> LoadAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Removes wizard state from storage.
    /// </summary>
    /// <param name="key">Unique identifier for the wizard state</param>
    /// <param name="ct">Cancellation token</param>
    Task RemoveAsync(string key, CancellationToken ct = default);
}
