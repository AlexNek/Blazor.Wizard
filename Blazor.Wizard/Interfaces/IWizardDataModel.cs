namespace Blazor.Wizard.Interfaces;

/// <summary>
/// Marker interface for wizard data models that should be persisted.
/// Only types implementing this interface will be serialized during state persistence.
/// Services and other non-data objects should NOT implement this interface.
/// </summary>
public interface IWizardDataModel
{
}
