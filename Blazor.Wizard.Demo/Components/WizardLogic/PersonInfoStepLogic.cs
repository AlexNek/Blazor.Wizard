using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic;

public sealed class PersonInfoStepLogic : GeneralStepLogic<PersonInfoModel>
{
    public string DemoParameter { get; }
    public override Type Id => typeof(PersonInfoStepLogic);

    public PersonInfoStepLogic(string demoParameter)
    {
        DemoParameter = demoParameter;
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

        if (person.Age < AgeRuleConstants.MinAllowedAge)
        {
            validation.IsValid = false;
            validation.ErrorMessage = "Age must be at least 16 to proceed.";
            AddValidationError(editContext, nameof(PersonInfoModel.Age), validation.ErrorMessage);
            NotifyValidation(editContext);
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        NotifyValidation(editContext);
        return new StepResult { NextStepId = typeof(AddressStepLogic), StayOnStep = false, CanContinue = true };
    }
}