using Blazor.Wizard.Core;
using Blazor.Wizard.DemoDevEx.Models;
using Blazor.Wizard.Interfaces;

namespace Blazor.Wizard.DemoDevEx.Components.Wizard;

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
        var modelInstance = GetModel();
        var editContextModel = editContext?.Model;
        System.Diagnostics.Debug.WriteLine($"[DEBUG] GetModel == EditContext.Model: {object.ReferenceEquals(modelInstance, editContextModel)}");
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
            return new StepResult { StayOnStep = true, CanContinue = false };
        }

        NotifyValidation(editContext);
        return new StepResult { NextStepId = typeof(AddressStepLogic), StayOnStep = false, CanContinue = true };
    }
}