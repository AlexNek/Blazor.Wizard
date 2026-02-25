using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public class StepContext
{
    public QuestionaryModel Model { get; init; } = new();
    public IServiceProvider? Services { get; init; }
}