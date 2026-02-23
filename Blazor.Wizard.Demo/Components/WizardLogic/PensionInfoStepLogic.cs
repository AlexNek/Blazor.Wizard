using Blazor.Wizard.Demo.Models;

namespace Blazor.Wizard.Demo.Components.WizardLogic;

public sealed class PensionInfoStepLogic : BaseStepLogic<AddressModel>
{
    private PersonInfoModel? _cachedPersonInfo;
    public override Type Id => typeof(PensionInfoStepLogic);
    public override bool IsVisible => ShouldShowPension(_cachedPersonInfo);

    public override StepResult Evaluate(IWizardData data, ValidationResult validation)
    {
        if (!validation.IsValid)
        {
            return new StepResult { StayOnStep = true };
        }

        return new StepResult { NextStepId = typeof(SummaryStepLogic), StayOnStep = false, CanContinue = true };
    }

    public void UpdatePersonInfo(PersonInfoModel? personInfo)
    {
        _cachedPersonInfo = personInfo;
    }

    private static bool ShouldShowPension(PersonInfoModel? personInfo)
    {
        // Show pension step only if age > AgeRuleConstants.MaxPensionAge
        if (personInfo == null)
        {
            return false;
        }

        return personInfo.Age > AgeRuleConstants.MaxPensionAge;
    }

    //public EditContext GetEditContext() => Context;
    //public AddressModel GetModel() => Model;
}