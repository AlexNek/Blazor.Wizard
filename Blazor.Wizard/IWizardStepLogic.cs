namespace Blazor.Wizard;

public interface IWizardStepLogic
{
    Task<bool> CanLeaveAsync();
    Task OnEnterAsync();
    Task<bool> OnFinishAsync();
}