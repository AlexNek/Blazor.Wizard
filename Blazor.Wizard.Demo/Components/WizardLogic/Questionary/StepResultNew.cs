namespace Blazor.Wizard.Demo.Components.WizardLogic.Questionary;

public record StepResultNew(bool Success, QuestionaryStepId? NextStepOverride = null, string? ErrorMessage = null, object? Payload = null);