namespace Blazor.Wizard;

public sealed class WizardTransitionState
{
    public string? BlockReason { get; }

    public bool CanProceed { get; }

    public WizardTransitionState(bool canProceed, string? blockReason = null)
    {
        CanProceed = canProceed;
        BlockReason = blockReason;
    }
}
