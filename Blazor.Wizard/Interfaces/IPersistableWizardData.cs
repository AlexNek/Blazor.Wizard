namespace Blazor.Wizard.Interfaces;

/// <summary>
/// Extends IWizardData with persistence capabilities.
/// </summary>
public interface IPersistableWizardData : IWizardData
{
    Dictionary<Type, object> GetAllData();
}
