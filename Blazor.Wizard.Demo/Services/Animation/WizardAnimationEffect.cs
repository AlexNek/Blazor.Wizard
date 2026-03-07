namespace Blazor.Wizard.Demo.Services.Animation;

public sealed record WizardAnimationEffect(
    Guid Id,
    string Emoji,
    string Text,
    string CssClass,
    double LeftPercent);
