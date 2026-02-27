using Blazor.Wizard.Core;
using Blazor.Wizard.DemoDevEx.Models;
using Blazor.Wizard.Interfaces;
using Blazor.Wizard.ViewModels;

using Microsoft.AspNetCore.Components.Forms;

namespace Blazor.Wizard.DemoDevEx.Components.Wizard;

public class PersonWizardViewModel : ComponentWizardViewModel<PersonModel>
{
    public PersonWizardViewModel(
        PersonModelMapper modelMapper,
        IWizardDiagnostics? diagnostics = null) 
        : base(modelMapper, diagnostics)
    {
    }

    protected override Type ResolveComponentType(IWizardStep step)
    {
        return PersonStepRegistry.GetByStepIdType(step.Id).ComponentType;
    }

    protected override IReadOnlyList<Func<IWizardStep>> GetDefaultStepFactories()
    {
        return PersonStepRegistry.CreateStepFactories();
    }

    public override void Initialize(IEnumerable<Func<IWizardStep>>? stepFactories)
    {
        base.Initialize(stepFactories);

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
            await UpdateCanProceedAsync();

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
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"Exception in OnFieldChanged: {ex}");
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