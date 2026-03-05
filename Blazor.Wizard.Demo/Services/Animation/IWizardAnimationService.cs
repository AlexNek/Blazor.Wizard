namespace Blazor.Wizard.Demo.Services.Animation;

public interface IWizardAnimationService
{
    event Action<WizardAnimationEffect>? EffectTriggered;

    void Celebrate(string text);
    void Warn(string text);
}
