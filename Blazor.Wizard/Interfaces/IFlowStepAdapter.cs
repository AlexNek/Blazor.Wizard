namespace Blazor.Wizard.Interfaces;

public interface IFlowStepAdapter
{
    Task OnEnterAsync();
    Task<bool> CanLeaveAsync();
    Task<bool> OnFinishAsync();
}
