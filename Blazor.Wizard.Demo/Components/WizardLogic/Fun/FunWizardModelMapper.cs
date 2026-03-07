using Blazor.Wizard.Demo.Models.Fun;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Fun;

public class FunWizardModelMapper : IWizardModelBuilder<FunWizardResult>, IWizardModelSplitter<FunWizardResult>
{
    public FunWizardResult Build(IWizardData data)
    {
        if (!data.TryGet<FunMoodStepModel>(out var moodStep) || moodStep == null)
        {
            throw new InvalidOperationException("Missing FunMoodStepModel data.");
        }

        if (!data.TryGet<FunSnackStepModel>(out var snackStep) || snackStep == null)
        {
            throw new InvalidOperationException("Missing FunSnackStepModel data.");
        }

        return new FunWizardResult
        {
            Mood = moodStep.Mood,
            Tacos = snackStep.Tacos,
            Donuts = snackStep.Donuts,
            Popcorn = snackStep.Popcorn
        };
    }

    public void Split(FunWizardResult result, IWizardData data)
    {
        data.Set(new FunMoodStepModel
        {
            Mood = result.Mood
        });

        data.Set(new FunSnackStepModel
        {
            Tacos = result.Tacos,
            Donuts = result.Donuts,
            Popcorn = result.Popcorn
        });
    }
}
