namespace Blazor.Wizard;

public interface IFlowStepAdapter
{
    Task OnEnterAsync();
    Task<bool> CanLeaveAsync();
    Task<bool> OnFinishAsync();
}
