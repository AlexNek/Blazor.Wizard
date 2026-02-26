using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Obsolete;

[Obsolete("Use IWizardModelMapper<TResult> instead. IWizardResultBuilder only supports Build, while IWizardModelMapper supports both Build and Split.")]
public interface IWizardResultBuilder<TResult>
{
    TResult Build(IWizardData data);
}