using Blazor.Wizard.Core;
using Blazor.Wizard.Demo.Models.Person;
using Blazor.Wizard.Demo.Services.Animation;
using Blazor.Wizard.Demo.Services.Toaster;
using Blazor.Wizard.Extensions;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.Demo.Components.WizardLogic.Person;

public sealed class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    private readonly IWizardAnimationService _animationService;

    public string DemoParameter { get; } = "DI animation service enabled";
    public override Type Id => typeof(PersonInfoStepLogic);

    public PersonInfoStepLogic(IWizardAnimationService animationService)
    {
        _animationService = animationService;
    }

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        var editContext = GetEditContext();
        EnsureValidationMessageStore(editContext);
        ClearValidation(editContext, nameof(PersonInfoModel.Age));

        PersonInfoModel? person = null;
        if (!data.TryGet(out person) || person == null)
        {
            return new StepResult { StayOnStep = true };
        }

        var toaster = data.GetService<IToasterService>();

        if (person.Age < AgeRuleConstants.MinAllowedAge)
        {
            _animationService.Warn("Too young for wizard mission");
            toaster.ShowWarning("Age must be at least 16 to proceed.");

            validation.IsValid = false;
            validation.ErrorMessage = "Age must be at least 16 to proceed.";
            AddValidationError(editContext, nameof(PersonInfoModel.Age), validation.ErrorMessage);
            NotifyValidation(editContext);
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        _animationService.Celebrate("Level up! Next step unlocked");

        toaster.ShowSuccess("Person info validated. Moving to address step.");

        NotifyValidation(editContext);
        return new StepResult { NextStepId = typeof(AddressStepLogic), StayOnStep = false, CanContinue = true };
    }
}
