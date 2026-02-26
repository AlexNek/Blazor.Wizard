namespace Blazor.Wizard.Obsolete;

[Obsolete("IWizardStepLogic is deprecated and not used in the current architecture. Use IWizardStep interface instead.", false)]
public interface IWizardStepLogic
{
    Task<bool> CanLeaveAsync();
    Task OnEnterAsync();
    Task<bool> OnFinishAsync();
}