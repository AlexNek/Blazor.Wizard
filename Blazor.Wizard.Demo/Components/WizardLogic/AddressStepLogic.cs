using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic;

public sealed class AddressStepLogic : BaseStepLogic<AddressModel>
{
    public override Type Id => typeof(AddressStepLogic);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        // Decide next step at runtime based on age
        if (data.TryGet<PersonInfoModel>(out var person) && person != null &&
            person.Age > AgeRuleConstants.MaxPensionAge)
        {
            return new StepResult { NextStepId = typeof(PensionInfoStepLogic), StayOnStep = false, CanContinue = true };
        }

        return new StepResult { NextStepId = typeof(SummaryStepLogic), StayOnStep = false, CanContinue = true };
    }
}