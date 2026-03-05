namespace Blazor.Wizard.Demo.Services.Animation;

public sealed class WizardAnimationService : IWizardAnimationService
{
    public event Action<WizardAnimationEffect>? EffectTriggered;

    public void Celebrate(string text)
    {
        Trigger("🎉", text, "effect-celebrate");
    }

    public void Warn(string text)
    {
        Trigger("🤡", text, "effect-warn");
    }

    private void Trigger(string emoji, string text, string cssClass)
    {
        var effect = new WizardAnimationEffect(
            Guid.NewGuid(),
            emoji,
            text,
            cssClass,
            Random.Shared.Next(12, 88));

        EffectTriggered?.Invoke(effect);
    }
}
