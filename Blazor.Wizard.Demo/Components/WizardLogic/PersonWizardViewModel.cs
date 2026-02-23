using System.Diagnostics;
using Blazor.Wizard.Demo.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.Demo.Components.WizardLogic;

public class PersonWizardViewModel : WizardViewModel<IWizardStep, WizardData, PersonModel>
{
    private readonly WizardStepFactory _factory = new();

    public PersonWizardViewModel() : base(new PersonModelResultBuilder())
    {
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        _factory.Register(typeof(PersonInfoStepLogic), () => new PersonInfoStepLogic("Demo value from factory"));
        _factory.Register(typeof(AddressStepLogic), () => new AddressStepLogic());
        _factory.Register(typeof(PensionInfoStepLogic), () => new PensionInfoStepLogic());
        _factory.Register(typeof(SummaryStepLogic), () => new SummaryStepLogic());

        var effectiveFactories = stepFactories != null && stepFactories.Any()
            ? stepFactories
            : new List<Func<IWizardStep>>
            {
                () => _factory.CreateStep(typeof(PersonInfoStepLogic)),
                () => _factory.CreateStep(typeof(AddressStepLogic)),
                () => _factory.CreateStep(typeof(PensionInfoStepLogic)),
                () => _factory.CreateStep(typeof(SummaryStepLogic))
            };

        base.Initialize(effectiveFactories);

        // Update PensionInfoStepLogic visibility based on PersonInfoModel
        var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
        var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
        if (personStep != null && pensionStep != null)
        {
            if (Data.TryGet<PersonInfoModel>(out var personInfo))
            {
                pensionStep.UpdatePersonInfo(personInfo);
            }
        }
    }

    public override async Task<bool> NextAsync()
    {
        UpdatePensionInfoIfNeeded();
        return await base.NextAsync();
    }

    /// <summary>
    ///     WARNING: async void is required here because EditContext.OnFieldChanged expects a void event handler.
    ///     All exceptions are caught and logged to avoid unhandled errors.
    /// </summary>
    protected override async void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        try
        {
            // Run business logic validation live for PersonInfoStepLogic
            if (Flow != null && Steps.Count > 0 && Flow.Index >= 0 && Flow.Index < Steps.Count)
            {
                var step = Steps[Flow.Index];
                if (step is PersonInfoStepLogic personStep)
                {
                    var validation = new ValidationResult { IsValid = await personStep.ValidateAsync(Data) };
                    personStep.Evaluate(Data, validation);
                }
            }

            // Update CanProceed AFTER business logic validation has run
            // This ensures validation errors are cleared/added before we check the state
            await UpdateCanProceedAsync();
        }
        catch (Exception ex)
        {
            Trace.TraceError($"Exception in OnFieldChanged: {ex}");
        }
    }

    private void UpdatePensionInfoIfNeeded()
    {
        var personStep = Steps.OfType<PersonInfoStepLogic>().FirstOrDefault();
        var pensionStep = Steps.OfType<PensionInfoStepLogic>().FirstOrDefault();
        if (personStep != null && pensionStep != null)
        {
            if (Data.TryGet<PersonInfoModel>(out var personInfo))
            {
                pensionStep.UpdatePersonInfo(personInfo);
            }
        }
    }
}