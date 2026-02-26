namespace Blazor.Wizard;

public class WizardEvent
{
    public string EventType { get; }

    public string StepName { get; }

    public WizardEvent(string eventType, string stepName)
    {
        EventType = eventType;
        StepName = stepName;
    }
}
